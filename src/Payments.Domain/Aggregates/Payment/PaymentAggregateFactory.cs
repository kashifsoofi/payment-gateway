namespace Payments.Domain.Aggregates.Payment
{
    using System;

    public class PaymentAggregateFactory : IPaymentAggregateFactory
    {
        public PaymentAggregate Create(Guid id)
        {
            return new PaymentAggregate(id);
        }

        public PaymentAggregate Create(PaymentAggregateState state)
        {
            return new PaymentAggregate(state);
        }
    }
}
