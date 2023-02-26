namespace Payments.Contracts.Messages.Events
{
    using Payments.Contracts.Enums;
    using System;

    public class PaymentCreated : AggregateEvent, IPaymentCreated
    {
        public Guid Id { get; }
        public Guid MerchantId { get; }
        public string CardHolderName { get; }
        public string CardNumber { get; }
        public int ExpiryMonth { get; }
        public int ExpiryYear { get; }
        public decimal Amount { get; }
        public string CurrencyCode { get; }
        public string Reference { get; }
        public PaymentStatus Status { get; }

        public PaymentCreated(
            Guid id,
            Guid merchantId,
            string cardHolderName,
            string cardNumber,
            int expiryMonth,
            int expiryYear,
            decimal amount,
            string currencyCode,
            string reference,
            PaymentStatus status)
        {
            Id = id;
            MerchantId = merchantId;
            CardHolderName = cardHolderName;
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            Amount = amount;
            CurrencyCode = currencyCode;
            Reference = reference;
            Status = status;
        }
    }
}
