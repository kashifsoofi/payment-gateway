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
        private readonly DatabaseHelper<Guid, PaymentAggregateState> aggregateNameDatabaseHelper;

        private readonly GetAllPaymentsQuery sut;

        public GetAllPaymentsQueryTests(DatabaseFixture databaseFixture)
        {
            var connectionStringProvider = databaseFixture.ConnectionStringProvider;

            aggregateNameDatabaseHelper = new DatabaseHelper<Guid, PaymentAggregateState>("Payment", connectionStringProvider.PaymentsConnectionString, x => x.Id);

            this.sut = new GetAllPaymentsQuery(connectionStringProvider);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await aggregateNameDatabaseHelper.CleanTableAsync();
        }

        [Fact(Skip = "TestContainers need updating")]
        public async Task ExecuteAsync_GivenNoRecords_ShouldReturnEmptyCollection()
        {
            // Arrange
            var result = await this.sut.ExecuteAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Theory(Skip = "TestContainers need updating")]
        [AutoData]
        public async Task ExecuteAsync_GivenRecordsExist_ShouldReturnRecords(List<PaymentAggregateState> states)
        {
            // Arrange
            await this.aggregateNameDatabaseHelper.AddRecordsAsync(states);

            // Act
            var result = await this.sut.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(states);
        }
    }
}
