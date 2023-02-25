namespace Payments.Contracts.Messages.Events
{
    using System;

    public interface IPaymentCreated
    {
        Guid Id { get; }
        DateTime CreatedOn { get; }
    }
}
