using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface IPaymentService
    {
        Task<Result<string>> CreatePaymentLinkAsync(int orderId, decimal amount, string currency, string userEmail);

        // 2. معالجة الـ Webhook (لاستقبال تحديث حالة الدفع من Paymob)
        // Paymob ترسل بيانات كثيرة في الـ Webhook.
        Task<Result> HandleWebhookCallbackAsync(Dictionary<string, string> callbackData);
    }
}
