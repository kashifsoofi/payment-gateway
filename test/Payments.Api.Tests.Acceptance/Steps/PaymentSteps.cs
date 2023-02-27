using FluentAssertions;
using PaymentGateway.ApiClient;
using PaymentGateway.ApiClient.Requests;
using PaymentGateway.ApiClient.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Payments.Api.Tests.Acceptance.Steps
{
    [Binding]
    public class PaymentSteps
    {
        private readonly ScenarioContext context;
        private readonly PaymentGatewayApiClient paymentGatewayApiClient;

        public PaymentSteps(ScenarioContext context, PaymentGatewayApiClient paymentGatewayApiClient)
        {
            this.context = context;
            this.paymentGatewayApiClient = paymentGatewayApiClient;
        }

        [Given(@"a valid merchant id")]
        public void GivenAValidMerchantId()
        {
            context.Set(Guid.NewGuid(), "MerchantId");
        }

        [Given(@"cardNumber '([^']*)' and reference '([^']*)'")]
        public void GivenCardNumberAndReference(string cardNumber, string reference)
        {
            context.Set(cardNumber, "CardNumber");
            context.Set(reference, "Reference");
        }

        [When(@"the client creates a payment")]
        public async Task WhenTheClientCreatesAPayment()
        {
            context.Set(Guid.NewGuid(), "PaymentId");
            var createPaymentRequest = new CreatePaymentRequest
            {
                Id = context.Get<Guid>("PaymentId"),
                CardHolderName = "Chuck Norris",
                CardNumber = context.Get<string>("CardNumber"),
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Cvv = "123",
                Amount = 100,
                CurrencyCode = "GBP",
                Reference = context.Get<string>("Reference"),
            };

            var result = await paymentGatewayApiClient.CreatePaymentAsync(context.Get<Guid>("MerchantId"), createPaymentRequest);
            context.Set(result, "Result");
        }

        [Then(@"create payment result should be success")]
        public void ThenCreatePaymentResultShouldBeSuccess()
        {
            var result = context.Get<bool>("Result");
            result.Should().Be(true);
        }

        [Given(@"a payment exists for a merchant")]
        public async Task GivenAPaymentExistsForAMerchant()
        {
            var merchantId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var createPaymentRequest = new CreatePaymentRequest
            {
                Id = paymentId,
                CardHolderName = "Chuck Norris",
                CardNumber = "4242424242424242",
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Cvv = "123",
                Amount = 100,
                CurrencyCode = "GBP",
                Reference = "REF0011",
            };

            var result = await paymentGatewayApiClient.CreatePaymentAsync(merchantId, createPaymentRequest);
            result.Should().Be(true);

            context.Set(merchantId, "MerchantIdForGet");
            context.Set(paymentId, "PaymentIdForGet");
        }

        [When(@"client gets a payment")]
        public async void WhenClientGetsAPayment()
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));
            var merchantId = context.Get<Guid>("MerchantIdForGet");
            var paymentId = context.Get<Guid>("PaymentIdForGet");

            var result = await paymentGatewayApiClient.GetPaymentByIdAsync(merchantId, paymentId);
            context.Set(result, "GetPaymentByIdResult");
        }

        [Then(@"payment should be returned")]
        public void ThenPaymentShouldBeReturned()
        {
            Thread.Sleep(TimeSpan.FromSeconds(20));
            var paymentResponse = context.Get<PaymentResponse>("GetPaymentByIdResult");
            paymentResponse.Should().NotBeNull();
            paymentResponse.MerchantId.Should().Be(context.Get<Guid>("MerchantIdForGet"));
            paymentResponse.Id.Should().Be(context.Get<Guid>("PaymentIdForGet"));
        }
    }
}
