namespace Payments.Domain.Aggregates.Payment
{
    using Payments.Contracts.Enums;
    using System;

    public interface IPaymentAggregateState
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
        DateTime CreatedOn { get; }
        DateTime UpdatedOn { get; }
    }
}
