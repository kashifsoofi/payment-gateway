namespace Payments.Domain.Aggregates.Payment
{
    using System.Threading.Tasks;

    public interface IPaymentAggregateWriteRepository
    {
        Task CreateAsync(IPaymentAggregate aggregate);

        Task UpdateAsync(IPaymentAggregate aggregate);

        Task DeleteAsync(IPaymentAggregate aggregate);
    }
}
