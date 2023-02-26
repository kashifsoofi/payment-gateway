namespace Payments.Domain.Aggregates.Payment
{
    using System;
    using System.Collections.Generic;
    using Payments.Contracts.Messages.Commands;
    using Payments.Contracts.Messages.Events;

    public interface IPaymentAggregate
    {
        Guid Id { get; }

        IPaymentAggregateState State { get; }

        bool IsNew { get; }

        void Create(CreatePayment command);

        List<IAggregateEvent> UncommittedEvents { get; set; }
    }
}