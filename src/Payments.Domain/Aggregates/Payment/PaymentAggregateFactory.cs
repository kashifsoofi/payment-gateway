namespace Payments.Domain.Aggregates.Payment
{
    using Payments.Domain.AcquiringBank;
    using System;

    public class PaymentAggregateFactory : IPaymentAggregateFactory
    {
        private readonly IAcquiringBankService acquiringBankService;

        public PaymentAggregateFactory(IAcquiringBankService acquiringBankService)
        {
            this.acquiringBankService = acquiringBankService;
        }

        public PaymentAggregate Create(Guid id)
        {
            return new PaymentAggregate(
                acquiringBankService,
                id);
        }

        public PaymentAggregate Create(PaymentAggregateState state)
        {
            return new PaymentAggregate(
                acquiringBankService,
                state);
        }
    }
}
