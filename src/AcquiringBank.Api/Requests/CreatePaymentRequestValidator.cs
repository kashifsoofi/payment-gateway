namespace AcquiringBank.Api.Requests
{
    using FluentValidation;

    public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentRequestValidator()
        {
            RuleFor(model => model.Id).NotEmpty();
            RuleFor(model => model.CardHolderName).MaximumLength(255).NotEmpty();
            RuleFor(model => model.CardNumber).CreditCard();
            RuleFor(model => model.ExpiryMonth).InclusiveBetween(1, 12);
            RuleFor(model => model.ExpiryYear).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Year);
            RuleFor(model => model.Cvv).MinimumLength(3).MaximumLength(4).Must(cvv =>
            {
                foreach (var c in cvv)
                {
                    if (c < '0' || c > '9')
                    {
                        return false;
                    }
                }
                return true;
            });
            RuleFor(model => model.Amount).NotEmpty().Must(amount => amount > 0);
            RuleFor(model => model.CurrencyCode).NotEmpty().Must(cur =>
            {
                cur = cur.ToUpper();
                return cur == "GBP";
            }).WithMessage("Only GBP is supported.");
            RuleFor(model => model.Reference).NotEmpty().MaximumLength(50);
        }
    }
}
