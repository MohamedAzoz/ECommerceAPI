using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;
using ECommerce.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using static ECommerce.Core.Models.PaymobModel;

namespace ECommerce.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        // نحتاج إلى الوصول إلى معلومات الطلب من قاعدة البيانات لتفاصيل المنتج
        private readonly IUnitOfWork unitOfWork;

        // بيانات Paymob Secret Keys من appsettings.json
        private readonly string _apiKey;
        private readonly string _integrationId;
        private readonly string _iframeId;

        public PaymentService(HttpClient httpClient, IConfiguration config, IUnitOfWork _unitOfWork)
        {
            _httpClient = httpClient;
            _config = config;
            unitOfWork = _unitOfWork;

            // قراءة الإعدادات من ملف appsettings.json
            _apiKey = config["Paymob:ApiKey"] ?? throw new ArgumentNullException("Paymob:ApiKey not found.");
            _integrationId = config["Paymob:IntegrationId"] ?? throw new ArgumentNullException("Paymob:IntegrationId not found.");
            _iframeId = config["Paymob:IFrameId"] ?? throw new ArgumentNullException("Paymob:IFrameId not found.");

            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/api/");
        }

        // المنطق الفعلي لإنشاء رابط الدفع
        public async Task<Result<string>> CreatePaymentLinkAsync(int orderId, decimal amount,
                    string currency, string userEmail)
        {
            // نحتاج إلى الحصول على الطلب وتفاصيل المستخدم من قاعدة البيانات هنا
            // ... (كود جلب بيانات الطلب) ...
            var result = await unitOfWork.Orders.FindAsync((x) => x.Id == orderId);
            if (!result.IsSuccess)
            {
                return Result<string>.Failure($"Orders with ID {orderId} not found.", 404);
            }
            var OrderItems = await unitOfWork.OrderItems.FindAllWithIncludeAsync((x) => x.OrderId == orderId,p=>p.Product);
            if (!OrderItems.IsSuccess)
            {
                return Result<string>.Failure("No Order Items found.",400);
            }
            var order = result.Value;
            var orderItems = OrderItems.Value;
            var paymobItems = orderItems.Select(item => new
            {
                name = item.Product.Name, // يجب التأكد أن Product تم تضمينه في جلب OrderItems
                amount_cents = (int)(item.Price * 100),
                quantity = item.Quantity
            }).ToList();
            
            var userResult = await unitOfWork.Users.FindAsync((x) => x.Id == order.UserId);
            if (!userResult.IsSuccess)
            {
                return Result<string>.Failure($"User with ID {order.UserId} not found.", 404);
            }
            var user = userResult.Value;

            // 1. Authentication: الحصول على الـ Token
            var authRequest = new { api_key = _apiKey };
                var authResponse = await _httpClient.PostAsJsonAsync("auth/tokens", authRequest);
                if (!authResponse.IsSuccessStatusCode) return Result<string>.Failure("Paymob Auth Failed.", 503);

            var authResponseData = await authResponse.Content.ReadFromJsonAsync<PaymobAuthResponse>();
            var authToken = authResponseData?.Token;
            if (authToken == null) return Result<string>.Failure("Paymob Auth Token is null.", 503);

            // 2. Order Registration: تسجيل الطلب لدى Paymob
            var orderRequest = new
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = (int)(order.TotalAmount * 100), // المبلغ بالقرش
                merchant_order_id = orderId, // ربط الطلب بمعرف الـ DB
                items = paymobItems // يفضل إرسال تفاصيل المنتجات هنا
            };
            var orderResponse = await _httpClient.PostAsJsonAsync("ecommerce/orders", orderRequest);
            if (!orderResponse.IsSuccessStatusCode) return Result<string>.Failure("Paymob Order Registration Failed.", 503);

            var paymobOrderData = await orderResponse.Content.ReadFromJsonAsync<PaymobOrderResponse>();
            var paymobOrderId = paymobOrderData?.Id;

            // 3. Payment Key: الحصول على مفتاح الدفع
            var paymentKeyRequest = new
            {
                auth_token = authToken,
                amount_cents = (int)(order.TotalAmount * 100),
                expiration = 3600, // صالح لمدة ساعة
                    order_id = paymobOrderId,
                    billing_data = new
                    {
                        apartment = "NA", // بيانات الفواتير مطلوبة
                        email = user.Email,
                        floor = "NA",
                        first_name = user.FirstName,
                        street = "NA",
                        building = "NA",
                        phone_number = "NA",
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "NA",
                        country = "NA",
                        last_name = user.LastName,
                        state = "NA"
                    },
                    currency = currency,
                    integration_id = int.Parse(_integrationId)
                };

                var paymentKeyResponse = await _httpClient.PostAsJsonAsync("acceptance/payment_keys", paymentKeyRequest);
                if (!paymentKeyResponse.IsSuccessStatusCode) return Result<string>.Failure("Paymob Payment Key Failed.", 503);

            var paymentToken = (await paymentKeyResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>())?["token"];
            // 4. Redirection Link: بناء رابط التوجيه (Iframe)
            var paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_iframeId}?payment_token={paymentToken}";

                return Result<string>.Success(paymentUrl);
        }

        // في ECommerce.Infrastructure.Services/PaymentService.cs

        public async Task<Result> HandleWebhookCallbackAsync(Dictionary<string, string> callbackData)
        {
            if (!callbackData.TryGetValue("hmac", out var receivedHmac))
            {
                return Result.Failure("HMAC signature missing in callback data.", 400);
            }

            // القائمة الرسمية لـ Paymob بترتيب أبجدي (للتأكد من أننا نغطي كل شيء)
            var requiredFields = new List<string>
    {
        "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
        "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_fee_collected",
        "is_refunded", "is_standalone_payment", "is_voided", "order", "owner",
        "pending", "source_data_pan", "source_data_sub_type", "source_data_type",
        "success" // هذا هو الحقل الأخير في الترتيب الأبجدي
    };

            var dataToHash = new List<string>();

            // 1. جمع القيم بترتيب أبجدي
            foreach (var field in requiredFields)
            {
                // Paymob تصر على استخدام القيمة كما أرسلت، حتى لو كانت فارغة/غير موجودة
                // يجب أن تكون القائمة التي أرسلتها سابقًا تحتوي على جميع الحقول المطلوبة
                if (callbackData.TryGetValue(field, out var value))
                {
                    dataToHash.Add(value);
                }
                else
                {
                    // إذا كان الحقل مفقودًا، قد يشير ذلك إلى مشكلة، أو قد يكون غير مطبق للمعاملة
                    // الأمان يتطلب أن نفترض القيمة الفارغة (string.Empty) إذا كانت مفقودة.
                    dataToHash.Add("");
                }
            }

            // 2. بناء سلسلة البيانات المفصولة
            var concatenatedString = string.Join("", dataToHash);

            // 3. حساب الـ Hash المحلي (HMAC-SHA512)
            var secret = _config["Paymob:HMAC_Secret"] ?? throw new InvalidOperationException("Paymob HMAC_Secret not configured.");
            var calculatedHmac = CalculateHMACSHA512(concatenatedString, secret);

            // 4. مقارنة التوقيعات (Verification)
            if (calculatedHmac.ToLower() != receivedHmac.ToLower())
            {
                return Result.Failure("HMAC verification failed. Possible fraud attempt.", 403);
            }

            // 5. التحقق من حالة الدفع وتحديث DB
            if (callbackData.TryGetValue("success", out var successValue) && successValue == "true")
            {
                if (!callbackData.TryGetValue("merchant_order_id", out var orderIdStr) ||
                    !int.TryParse(orderIdStr, out var orderId))
                {
                    return Result.Failure("Missing or invalid merchant order ID.", 400);
                }

                // استخدام "Payment Received" أو "Processing" كحالة نجاح
                var updateResult = await UpdateOrderStatusInDb(orderId, "Paid");

                if (updateResult.IsFailure)
                {
                    return Result.Failure("Payment succeeded but failed to update local order status.", 500);
                }

                return Result.Success();
            }
            else
            {
                // الدفع فشل
                if (callbackData.TryGetValue("merchant_order_id", out var orderIdStr) &&
                    int.TryParse(orderIdStr, out var orderId))
                {
                    // تحديث حالة الطلب إلى "Failed" 
                    await UpdateOrderStatusInDb(orderId, "Failed");
                }
                return Result.Failure("Payment failed or was denied.", 402);
            }
        }

        // ------------------------------------------------------------------
        // الدوال المساعدة المطلوبة
        // ------------------------------------------------------------------

        // دالة لحساب HMAC-SHA512 (ضرورية)
        private string CalculateHMACSHA512(string data, string key)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            // تحويل الـ Hash إلى سلسلة سداسية عشرية
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        // دالة وهمية لتحديث DB (يجب استبدالها بمنطق UpdateStatus الحقيقي لديك)
        private async Task<Result> UpdateOrderStatusInDb(int orderId, string newStatus)
        {
            // استخدام وحدة العمل لتحديث الطلب
            var orderResult = await unitOfWork.Orders.FindAsync(o => o.Id == orderId);
            if (orderResult.IsSuccess)
            {
                orderResult.Value.Status = newStatus;
                unitOfWork.Orders.Update(orderResult.Value);
                await unitOfWork.Completed();
                return Result.Success();
            }
            return Result.Failure($"Order {orderId} not found in DB.", 404);
        }
    }
}
