namespace Payments.Infrastructure.AcquiringBank
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
