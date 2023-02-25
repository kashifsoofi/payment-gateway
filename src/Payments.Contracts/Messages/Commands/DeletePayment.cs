namespace Payments.Contracts.Messages.Commands
{
    using System;

    public class DeletePayment
    {
        public Guid Id { get; set; }

        public DeletePayment(Guid id)
        {
            Id = id;
        }
    }
}