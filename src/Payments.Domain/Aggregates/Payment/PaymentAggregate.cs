namespace Payments.Domain.Aggregates.Payment
{
    using System;
    using System.Collections.Generic;
    using Payments.Contracts.Messages.Commands;
    using Payments.Contracts.Messages.Events;

    public class PaymentAggregate : IPaymentAggregate
    {
        private readonly PaymentAggregateState state;
        private readonly bool isNew;

        public PaymentAggregate(Guid id)
        {
            this.state = new PaymentAggregateState { Id = id };
            this.isNew = true;
        }

        public PaymentAggregate(PaymentAggregateState state)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.isNew = false;
        }

        public Guid Id => this.state.Id;

        public IPaymentAggregateState State => this.state;

        public bool IsNew => isNew;

        public void Create(CreatePayment command)
        {
            throw new NotImplementedException();
        }

        public void Update(UpdatePayment command)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public List<IAggregateEvent> UncommittedEvents { get; set; } = new List<IAggregateEvent> { };
    }
}
