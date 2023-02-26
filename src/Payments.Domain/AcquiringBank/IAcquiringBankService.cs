using Payments.Contracts.Enums;

namespace Payments.Domain.AcquiringBank
{
    public interface IAcquiringBankService
    {
        Task<PaymentStatus> CreatePaymentAsync(CreatePaymentRequest request);
    }
}
