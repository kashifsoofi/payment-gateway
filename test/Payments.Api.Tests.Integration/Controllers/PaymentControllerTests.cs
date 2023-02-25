namespace Payments.Api.Tests.Integration.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Newtonsoft.Json;
    using Payments.Contracts.Responses;
    using Xunit;

    public class PaymentControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public PaymentControllerTests(WebApplicationFactory<Startup> factory)
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
            var response = await client.GetAsync($"api/aggregatename/{id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var aggregatename = JsonConvert.DeserializeObject<Payment>(responseContent);
            aggregatename.Id.Should().Be(id);
        }
    }
}
