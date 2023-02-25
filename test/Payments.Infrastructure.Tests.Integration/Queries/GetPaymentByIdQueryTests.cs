namespace Payments.Infrastructure.Tests.Integration.Queries
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using FluentAssertions;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.Queries;
    using Payments.Infrastructure.Tests.Integration.Helpers;
    using Xunit;

    [Collection("DatabaseCollection")]
    public class GetPaymentByIdQueryTests : IAsyncLifetime
    {
        private readonly DatabaseHelper<Guid, PaymentAggregateState> aggregateNameDatabaseHelper;

        private readonly GetPaymentByIdQuery sut;

        public GetPaymentByIdQueryTests(DatabaseFixture databaseFixture)
        {
            var connectionStringProvider = databaseFixture.ConnectionStringProvider;

            aggregateNameDatabaseHelper = new DatabaseHelper<Guid, PaymentAggregateState>("Payment", connectionStringProvider.PaymentsConnectionString, x => x.Id);

            this.sut = new GetPaymentByIdQuery(connectionStringProvider);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await aggregateNameDatabaseHelper.CleanTableAsync();
        }

        [Theory(Skip = "TestContainers need updating")]
        [AutoData]
        public async Task ExecuteAsync_GivenNoRecordExists_ShouldReturnNull(Guid id)
        {
            // Arrange
            var result = await this.sut.ExecuteAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Theory(Skip = "TestContainers need updating")]
        [AutoData]
        public async Task ExecuteAsync_GivenRecordExists_ShouldReturnPayment(PaymentAggregateState state)
        {
            // Arrange
            await this.aggregateNameDatabaseHelper.AddRecordAsync(state);

            // Act
            var result = await this.sut.ExecuteAsync(state.Id);

            // Assert
            result.Should().BeEquivalentTo(
                state,
                x => x
                    .Excluding(p => p.CreatedOn)
                    .Excluding(p => p.UpdatedOn));
        }
    }
}
