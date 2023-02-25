namespace Payments.Infrastructure.Queries
{
    using System;
    using System.Threading.Tasks;
    using Payments.Contracts.Responses;

    public interface IGetPaymentByIdQuery
    {
        Task<Payment> ExecuteAsync(Guid id);
    }
}
