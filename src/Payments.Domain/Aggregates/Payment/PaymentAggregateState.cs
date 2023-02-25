namespace Payments.Domain.Aggregates.Payment
{
    using System;

    public class PaymentAggregateState : IPaymentAggregateState
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
