namespace Payments.Domain.Aggregates.Payment
{
    using System;

    public interface IPaymentAggregateFactory
    {
        PaymentAggregate Create(Guid id);

        PaymentAggregate Create(PaymentAggregateState state);
    }
}
