namespace Payments.Contracts.Messages.Commands
{
    using System;

    public class CreatePayment
    {
        public Guid Id { get; set; }
        public Guid MerchantId { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Reference { get; set; }

        public CreatePayment(
            Guid id,
            Guid merchantId,
            string cardHolderName,
            string cardNumber,
            int expiryMonth,
            int expiryYear,
            string cvv,
            decimal amount,
            string currencyCode,
            string reference
            )
        {
            Id = id;
            MerchantId = merchantId;
            CardHolderName = cardHolderName;
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            Cvv = cvv;
            Amount = amount;
            CurrencyCode = currencyCode;
            Reference = reference;
        }
    }
}
