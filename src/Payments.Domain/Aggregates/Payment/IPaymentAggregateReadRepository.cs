namespace Payments.Domain.Aggregates.Payment
{
    using System;
    using System.Threading.Tasks;

    public interface IPaymentAggregateReadRepository
    {
        Task<IPaymentAggregate> GetByIdAsync(Guid id);
    }
}
