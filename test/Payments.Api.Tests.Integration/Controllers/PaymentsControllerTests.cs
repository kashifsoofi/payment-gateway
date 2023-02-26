namespace Payments.Api.Tests.Integration.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Newtonsoft.Json;
    using Payments.Contracts.Requests;
    using Payments.Contracts.Responses;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.Database;
    using Xunit;

    public class PaymentsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly HttpClient client;
        private readonly PaymentsDatabaseHelper paymentsDatabaseHelper;

        public PaymentsControllerTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory;
            client = factory.CreateClient();

            var connectionStringProvider = factory.Services.GetService(typeof(IConnectionStringProvider)) as IConnectionStringProvider;
            paymentsDatabaseHelper = new PaymentsDatabaseHelper(connectionStringProvider!.PaymentsConnectionString);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await paymentsDatabaseHelper.CleanTableAsync();
        }

        [Theory]
        [AutoData]
        public async Task Get_should_return_ok_with_Payment(PaymentAggregateState state)
        {
            // Arrange
            state.CardNumber = state.CardNumber.Substring(0, 4);
            state.CurrencyCode = "GBP";
            await this.paymentsDatabaseHelper.AddRecordAsync(state);
            var id = state.Id;

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/payments/{id}");
            request.Headers.Add("Merchant-Id", state.MerchantId.ToString());

            // Act
            var response = await client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var aggregatename = JsonConvert.DeserializeObject<Payment>(responseContent);
            aggregatename.Id.Should().Be(id);
        }

        [Theory]
        [AutoData]
        public async Task Post_GivenInvalidInput_ShouldReturnBadRequest(CreatePaymentRequest request)
        {
            // Arrange
            request.Id = Guid.Empty;
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/payments", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
