namespace Payments.Domain.AcquiringBank
{
    public class CreatePaymentRequest
    {
        public Guid Id { get; set; }
        public Guid MerchantId { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Reference { get; set; }
    }
}
