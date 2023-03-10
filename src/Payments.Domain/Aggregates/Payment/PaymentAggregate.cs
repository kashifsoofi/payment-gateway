namespace Payments.Domain.Aggregates.Payment
{
    using System;
    using System.Collections.Generic;
    using Payments.Contracts.Messages.Commands;
    using Payments.Contracts.Messages.Events;
    using Payments.Domain.AcquiringBank;

    public class PaymentAggregate : IPaymentAggregate
    {
        private readonly IAcquiringBankService acquiringBankService;

        private readonly PaymentAggregateState state;
        private readonly bool isNew;

        public PaymentAggregate(
            IAcquiringBankService acquiringBankService,
            Guid id)
        {
            this.acquiringBankService = acquiringBankService;

            this.state = new PaymentAggregateState { Id = id };
            this.isNew = true;
        }

        public PaymentAggregate(
            IAcquiringBankService acquiringBankService,
            PaymentAggregateState state)
        {
            this.acquiringBankService = acquiringBankService;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.isNew = false;
        }

        public Guid Id => this.state.Id;

        public IPaymentAggregateState State => this.state;

        public bool IsNew => isNew;

        public List<IAggregateEvent> UncommittedEvents { get; set; } = new List<IAggregateEvent> { };

        public async Task Create(CreatePayment command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (!isNew) throw new Exception("Payment already exists.");

            var status = await acquiringBankService.CreatePaymentAsync(
                new CreatePaymentRequest
                {
                    Id = command.Id,
                    MerchantId = command.MerchantId,
                    CardHolderName = command.CardHolderName,
                    CardNumber = command.CardNumber,
                    ExpiryMonth = command.ExpiryMonth,
                    ExpiryYear = command.ExpiryYear,
                    Cvv = command.Cvv,
                    Amount = command.Amount,
                    CurrencyCode = command.CurrencyCode,
                    Reference = command.Reference,
                });

            var evt = new PaymentCreated(
                command.Id,
                command.MerchantId,
                command.CardHolderName,
                command.CardNumber.Substring(command.CardNumber.Length - 4),
                command.ExpiryMonth,
                command.ExpiryYear,
                command.Amount,
                command.CurrencyCode,
                command.Reference,
                status);
            Apply(evt, Handle);
        }

        private void Apply<T>(T evt, Action<T> handler) where T : IAggregateEvent
        {
            handler(evt);
            this.UncommittedEvents.Add(evt);
        }

        private void Handle(PaymentCreated evt)
        {
            state.CreatedOn = evt.Timestamp;
            state.UpdatedOn = evt.Timestamp;
            state.MerchantId = evt.MerchantId;
            state.CardHolderName = evt.CardHolderName;
            state.CardNumber = evt.CardNumber;
            state.ExpiryMonth = evt.ExpiryMonth;
            state.ExpiryYear = evt.ExpiryYear;
            state.Amount = evt.Amount;
            state.CurrencyCode = evt.CurrencyCode;
            state.Reference = evt.Reference;
            state.Status = evt.Status;
        }
    }
}
