namespace Payments.Contracts.Messages.Events
{
    using System;

    public interface IAggregateEvent
    {
        DateTime Timestamp { get; set; }
    }
}