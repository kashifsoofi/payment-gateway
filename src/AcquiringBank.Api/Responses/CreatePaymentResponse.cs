namespace AcquiringBank.Api.Responses
{
    internal enum CreatePaymentResult
    {
        Success,
        InvalidCardNumber,
        InvalidExpiry,
        InsufficientFunds,
    }

    internal class CreatePaymentResponse
    {
        public CreatePaymentResult Result { get; set; }
    }
}
