using System.Text.Json.Serialization;

namespace PaymentGateway.ApiClient.Responses
{
    public enum PaymentStatus
    {
        Approved,
        Declined,
    }

    public class PaymentResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("merchantId")]
        public Guid MerchantId { get; set; }
        [JsonPropertyName("cardHolderName")]
        public string CardHolderName { get; set; }
        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; }
        [JsonPropertyName("expiryMonth")]
        public int ExpiryMonth { get; set; }
        [JsonPropertyName("expiryYear")]
        public int ExpiryYear { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
        [JsonPropertyName("reference")]
        public string Reference { get; set; }
        [JsonPropertyName("status")]
        public PaymentStatus Status { get; set; }
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }
        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}
