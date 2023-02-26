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

        private readonly PaymentsController sut;

        public PaymentControllerTests()
        {
            var messageSession = new TestableMessageSession();
            getAllPaymentsQueryMock = new Mock<IGetPaymentsByMerchantIdQuery>();
            getPaymentByIdQueryMock = new Mock<IGetPaymentByIdQuery>();

            sut = new PaymentsController(messageSession, getAllPaymentsQueryMock.Object, getPaymentByIdQueryMock.Object);
        }

        [Theory]
        [AutoData]
        public void Get_ShouldReturnOkAndPayments(List<Payment> aggregateNames)
        {
            // Arrange
            getAllPaymentsQueryMock
                .Setup(x => x.ExecuteAsync(Guid.Empty))
                .ReturnsAsync(aggregateNames);

            // Act
            var response = sut.Get();

            // Assert
            response.Should().NotBeNull();
            var okObjectResult = response.Result as OkObjectResult;
            okObjectResult.Should().NotBeNull();
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObjectResult.Value.Should().NotBeNull();
            okObjectResult.Value.Should().BeEquivalentTo(aggregateNames);

            getAllPaymentsQueryMock.Verify(x => x.ExecuteAsync(Guid.Empty), Times.Once);
        }

        [Fact]
        public async Task Get_GivenADefaultGuid_ShouldThrowException()
        {
            // Arrange
            var testValue = Guid.Empty;
            var defaultMessage = "Guid value cannot be default";
            var parameterName = "id";

            Func<Task> func = async () => await sut.Get(testValue);

            // Act & Assert
            var exception = await func.Should().ThrowAsync<ArgumentException>();
            exception.And.Message.Should().Contain(defaultMessage);
            exception.Which.ParamName.Should().Be(parameterName);
        }

        [Theory]
        [AutoData]
        public void Get_GivenRecordWithIdExists_ShouldReturnOkAndPayment(Payment payment)
        {
            // Arrange
            getPaymentByIdQueryMock
                .Setup(x => x.ExecuteAsync(payment.Id, payment.MerchantId))
                .ReturnsAsync(payment);

            // Act
            var response = sut.Get(payment.Id);

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
