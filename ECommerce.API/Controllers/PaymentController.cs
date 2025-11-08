using ECommerce.Core.DTOs.Common;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ------------------------------------------------------------------
        // A. نقطة نهاية إنشاء رابط الدفع (يستدعيها العميل)
        // المسار: POST /api/Payment/create-link
        // ------------------------------------------------------------------

        [HttpPost("create-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] PaymentRequestDto request)
        {
            // يُفضل أن ترسل هنا فقط OrderId, Currency, و UserEmail
            // ويتم حساب المبلغ (amount) داخل الخدمة لضمان أمان البيانات

            // تحقق من نموذج الإدخال (مثلاً استخدام FluentValidation)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // استدعاء الخدمة لإنشاء رابط الدفع
            var result = await _paymentService.CreatePaymentLinkAsync(
                request.OrderId,
                request.Amount, // يفضل جلب المبلغ من قاعدة البيانات لضمان عدم التلاعب به
                request.Currency,
                request.UserEmail
            );

            if (result.IsSuccess)
            {
                // إرجاع رابط التوجيه للدفع
                return Ok(new { paymentLink = result.Value });
            }

            // التعامل مع فشل التوثيق أو تسجيل الطلب لدى Paymob
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.InternalServerError, result.Error);
        }

        // ------------------------------------------------------------------
        // B. نقطة نهاية Webhook (تستدعيها Paymob)
        // المسار: GET/POST /api/paymob/callback
        // هذا المسار يجب أن يتطابق مع ما تم تعريفه في Paymob وفي Program.cs
        // ------------------------------------------------------------------

        // يجب أن تكون قادرة على استقبال GET أو POST (يفضل Paymob إرسالها كـ GET/Query)
        [HttpGet("/api/paymob/callback")]
        // أو [HttpPost("/api/paymob/callback")] حسب إعدادات Paymob
        public async Task<IActionResult> PaymobCallback([FromQuery] Dictionary<string, string> callbackData)
        {
            // Paymob ترسل بيانات المعاملة عبر Query String (مثلاً: ?hmac=...&success=...)
            // يتم تمرير جميع هذه البيانات إلى دالة HandleWebhookCallbackAsync

            var result = await _paymentService.HandleWebhookCallbackAsync(callbackData);

            if (result.IsSuccess)
            {
                // Paymob تتوقع رمز 200 OK عند نجاح المعالجة
                return Ok();
            }

            // في حالة فشل التحقق من HMAC أو فشل تحديث DB،
            // يفضل إرجاع رمز 400 أو 500 لتمكين Paymob من إعادة المحاولة (Retry)
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result.Error);
        }
    }
}
