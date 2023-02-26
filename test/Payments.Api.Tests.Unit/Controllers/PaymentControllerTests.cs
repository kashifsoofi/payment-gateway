namespace Payments.Api.Tests.Unit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using NServiceBus.Testing;
    using Xunit;
    using Payments.Api.Controllers;
    using Payments.Contracts.Responses;
    using Payments.Infrastructure.Queries;
    using Moq;
    using AutoFixture.Xunit2;

    public class PaymentControllerTests
    {
        private readonly Mock<IGetPaymentsByMerchantIdQuery> getAllPaymentsQueryMock;
        private readonly Mock<IGetPaymentByIdQuery> getPaymentByIdQueryMock;
        private readonly Mock<IGetPaymentByIdAndMerchantIdQuery> getPaymentByIdAndMerchantIdQueryMock;

        private readonly PaymentsController sut;

        public PaymentControllerTests()
        {
            var messageSession = new TestableMessageSession();
            getAllPaymentsQueryMock = new Mock<IGetPaymentsByMerchantIdQuery>();
            getPaymentByIdQueryMock = new Mock<IGetPaymentByIdQuery>();
            getPaymentByIdAndMerchantIdQueryMock = new Mock<IGetPaymentByIdAndMerchantIdQuery>();

            sut = new PaymentsController(messageSession, getAllPaymentsQueryMock.Object, getPaymentByIdQueryMock.Object, getPaymentByIdAndMerchantIdQueryMock.Object);
        }

        [Theory]
        [AutoData]
        public void Get_ShouldReturnOkAndPayments(Guid merchantId, List<Payment> payments)
        {
            // Arrange
            getAllPaymentsQueryMock
                .Setup(x => x.ExecuteAsync(merchantId))
                .ReturnsAsync(payments);

            // Act
            var response = sut.Get(merchantId);

            // Assert
            response.Should().NotBeNull();
            var okObjectResult = response.Result as OkObjectResult;
            okObjectResult.Should().NotBeNull();
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObjectResult.Value.Should().NotBeNull();
            okObjectResult.Value.Should().BeEquivalentTo(payments);

            getAllPaymentsQueryMock.Verify(x => x.ExecuteAsync(merchantId), Times.Once);
        }

        [Fact]
        public async Task Get_GivenADefaultGuid_ShouldReturnBadRequest()
        {
            // Arrange
            Guid merchantId = Guid.NewGuid();
            Guid id = Guid.Empty;

            // Act
            var response = await sut.Get(merchantId, id);

            // Act & Assert
            response.Should().NotBeNull();
            var badRequestObjectResult = response as BadRequestObjectResult;
            badRequestObjectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Theory]
        [AutoData]
        public void Get_GivenRecordWithIdExists_ShouldReturnOkAndPayment(Payment payment)
        {
            // Arrange
            getPaymentByIdAndMerchantIdQueryMock
                .Setup(x => x.ExecuteAsync(payment.Id, payment.MerchantId))
                .ReturnsAsync(payment);

            // Act
            var response = sut.Get(payment.MerchantId, payment.Id);

            // Assert
            response.Should().NotBeNull();
            var okObjectResult = response.Result as OkObjectResult;
            okObjectResult.Should().NotBeNull();
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObjectResult.Value.Should().NotBeNull();
            okObjectResult.Value.Should().BeEquivalentTo(payment);
        }
    }
}
