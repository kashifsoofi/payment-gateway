namespace Payments.Contracts.Messages.Commands
{
    using System;

    public class UpdatePayment
    {
        public Guid Id { get; set; }

        public UpdatePayment(Guid id)
        {
            Id = id;
        }
    }
}