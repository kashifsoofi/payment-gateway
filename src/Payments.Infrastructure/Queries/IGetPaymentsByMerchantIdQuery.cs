namespace Payments.Infrastructure.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Payments.Contracts.Responses;

    public interface IGetPaymentsByMerchantIdQuery
    {
        Task<IEnumerable<Payment>> ExecuteAsync(Guid merchantId);
    }
}
