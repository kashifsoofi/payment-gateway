namespace Payments.Contracts.Messages.Events
{
    using Payments.Contracts.Enums;
    using System;

    public interface IPaymentCreated : IAggregateEvent
    {
        Guid Id { get; }
        Guid MerchantId { get; }
        string CardHolderName { get; }
        string CardNumber { get; }
        int ExpiryMonth { get; }
        int ExpiryYear { get; }
        decimal Amount { get; }
        string CurrencyCode { get; }
        string Reference { get; }
        PaymentStatus Status { get; }
    }
}
