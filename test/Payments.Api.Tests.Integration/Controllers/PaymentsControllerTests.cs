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
    using Xunit;

    public class PaymentsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public PaymentsControllerTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Get_should_return_ok_with_Payment()
        {
            // Arrange
            var id = Guid.NewGuid();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync($"api/payments/{id}");

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
            var client = factory.CreateClient();

            // Act
            var response = await client.PostAsync("api/payments", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
