namespace Payments.Contracts.Messages.Events
{
    using System;

    public class AggregateEvent : IAggregateEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
