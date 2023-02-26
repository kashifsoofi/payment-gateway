namespace Payments.Contracts.Tests.Unit.Requests
{
    using AutoFixture.Xunit2;
    using FluentValidation.TestHelper;
    using Payments.Contracts.Requests;
    using System;
    using Xunit;

    public class CreatePaymentRequestValidatorTests
    {
        private readonly CreatePaymentRequestValidator sut = new CreatePaymentRequestValidator();

        [Theory]
        [AutoData]
        public void GivenEmptyId_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.Id = Guid.Empty;

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [AutoData]
        public void GivenEmptyCardHolderName_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.CardHolderName = "";

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CardHolderName);
        }

        [Theory]
        [AutoData]
        public void GivenInvalidCardNumber_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.CardNumber = "1234";

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CardNumber);
        }

        [Theory]
        [AutoData]
        public void GivenInvalidExpiryMonth_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.ExpiryMonth = 13;

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.ExpiryMonth);
        }

        [Theory]
        [AutoData]
        public void GivenInvalidExpiryYear_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.ExpiryYear = DateTime.Now.Year - 1;

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.ExpiryYear);
        }

        [Theory]
        [AutoData]
        public void GivenInvalidCvv_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.CardNumber = "4242424242424242";
            request.ExpiryMonth = 12;
            request.ExpiryYear = DateTime.Now.Year + 1;
            request.Cvv = "abc";

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Cvv);
        }

        [Theory]
        [AutoData]
        public void GivenInvalidCvvLength_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.CardNumber = "4242424242424242";
            request.ExpiryMonth = 12;
            request.ExpiryYear = DateTime.Now.Year + 1;
            request.Cvv = "12345";

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Cvv);
        }

        [Theory]
        [AutoData]
        public void GivenInvalidAmount_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.Amount = -1;

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        [Theory]
        [AutoData]
        public void GivenUnsupportedCurrency_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.CardNumber = "4242424242424242";
            request.ExpiryMonth = 12;
            request.ExpiryYear = DateTime.Now.Year + 1;
            request.Cvv = "123";
            request.CurrencyCode = "EUR";

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CurrencyCode);
        }

        [Theory]
        [AutoData]
        public void GivenInvalidReference_ShouldHaveValidationError(CreatePaymentRequest request)
        {
            request.CardNumber = "4242424242424242";
            request.ExpiryMonth = 12;
            request.ExpiryYear = DateTime.Now.Year + 1;
            request.Cvv = "123";
            request.CurrencyCode = "GBP";
            request.Reference = "";

            var result = sut.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Reference);
        }
    }
}
