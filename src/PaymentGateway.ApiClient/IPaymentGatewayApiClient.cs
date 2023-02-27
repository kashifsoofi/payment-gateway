namespace PaymentGateway.ApiClient
{
    using PaymentGateway.ApiClient.Requests;
    using PaymentGateway.ApiClient.Responses;

    public interface IPaymentGatewayApiClient
    {
        Task<bool> CreatePaymentAsync(Guid merchantId, CreatePaymentRequest request);
        Task<PaymentResponse> GetPaymentByIdAsync(Guid merchantId, Guid paymentId);
        Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync(Guid merchantId);
    }
}
