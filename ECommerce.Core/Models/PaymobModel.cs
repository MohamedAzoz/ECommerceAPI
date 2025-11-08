using System.Text.Json.Serialization;

namespace ECommerce.Core.Models
{
    public class PaymobModel
    {
        // في مجلد/ملف مثل Paymob.Models.cs
        // (يفضل استخدام C# 9+ records لسهولة القراءة)
        public record PaymobProfile(
            [property: JsonPropertyName("id")] int Id
        // أضف هنا أي خصائص أخرى داخل كائن Profile ترجعها Paymob
        );
        // 1. استجابة التوثيق (Authentication Response)
        public record PaymobAuthResponse(
            [property: JsonPropertyName("token")] string Token,
            [property: JsonPropertyName("profile")] PaymobProfile Profile
        );

        // 2. استجابة تسجيل الطلب (Order Registration Response)
        public record PaymobOrderResponse(
              [property: JsonPropertyName("id")] int Id,
              [property: JsonPropertyName("created_at")] string CreatedAt,
              [property: JsonPropertyName("pending")] bool Pending,
              [property: JsonPropertyName("amount_cents")] int AmountCents,
              [property: JsonPropertyName("merchant_order_id")] int MerchantOrderId
          );

        // 3. استجابة مفتاح الدفع (Payment Key Response) - (تم تعديل بسيط لتصحيح Profile)
        public record PaymobPaymentKeyResponse(
            [property: JsonPropertyName("token")] string Token,
            // 🌟 تم تغيير النوع من int إلى PaymobProfile
            [property: JsonPropertyName("profile")] PaymobProfile Profile,
            [property: JsonPropertyName("iframe_id")] int IframeId
        );
    }
}
