namespace Payments.Contracts.Messages.Commands
{
    using System;

    public class CreatePayment
    {
        public Guid Id { get; set; }

        public CreatePayment(Guid id)
        {
            Id = id;
        }
    }
}
