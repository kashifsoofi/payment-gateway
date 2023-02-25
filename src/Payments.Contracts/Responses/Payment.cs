namespace Payments.Contracts.Responses
{
    using System;

    public class Payment
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
