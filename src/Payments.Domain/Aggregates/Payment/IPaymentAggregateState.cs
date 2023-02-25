namespace Payments.Domain.Aggregates.Payment
{
    using System;

    public interface IPaymentAggregateState
    {
        Guid Id { get; }
        DateTime CreatedOn { get; }
        DateTime UpdatedOn { get; }
    }
}
