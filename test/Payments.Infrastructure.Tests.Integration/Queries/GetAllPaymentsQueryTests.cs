namespace Payments.Infrastructure.Tests.Integration.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using FluentAssertions;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.Queries;
    using Payments.Infrastructure.Tests.Integration.Helpers;
    using Xunit;

    [Collection("DatabaseCollection")]
    public class GetAllPaymentsQueryTests : IAsyncLifetime
    {
        private readonly PaymentsDatabaseHelper paymentsDatabaseHelper;

        private readonly GetPaymentsByMerchantIdQuery sut;

        public GetAllPaymentsQueryTests(DatabaseFixture databaseFixture)
        {
            var connectionStringProvider = databaseFixture.ConnectionStringProvider;

            paymentsDatabaseHelper = new PaymentsDatabaseHelper(connectionStringProvider.PaymentsConnectionString);

            this.sut = new GetPaymentsByMerchantIdQuery(connectionStringProvider);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await paymentsDatabaseHelper.CleanTableAsync();
        }

        [Fact]
        public async Task ExecuteAsync_GivenNoRecords_ShouldReturnEmptyCollection()
        {
            // Arrange
            var result = await this.sut.ExecuteAsync(Guid.Empty);

            // Assert
            result.Should().BeEmpty();
        }

        [Theory]
        [AutoData]
        public async Task ExecuteAsync_GivenRecordsExist_ShouldReturnRecords(List<PaymentAggregateState> states, Guid merchantId)
        {
            // Arrange
            foreach (var state in states)
            {
                state.MerchantId = merchantId;
                state.CardNumber = state.CardNumber.Substring(0, 4);
                state.CurrencyCode = "GBP";
            }
            await this.paymentsDatabaseHelper.AddRecordsAsync(states);

            // Act
            var result = await this.sut.ExecuteAsync(merchantId);

            // Assert
            result.Should().BeEquivalentTo(states, opts => opts.Excluding(x => x.CreatedOn).Excluding(x => x.UpdatedOn));
        }
    }
}
