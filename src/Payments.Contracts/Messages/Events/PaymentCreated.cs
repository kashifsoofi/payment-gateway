namespace Payments.Contracts.Messages.Events
{
    using System;

    public class PaymentCreated : IPaymentCreated
    {
        public Guid Id { get; }

        public DateTime CreatedOn { get; }

        public PaymentCreated(Guid id, DateTime createdOn)
        {
            this.Id = id;
            this.CreatedOn = createdOn;
        }
    }
}
