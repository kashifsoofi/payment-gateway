namespace Payments.Infrastructure.Tests.Integration.AggregateRepositories.Payment
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using FluentAssertions;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.AggregateRepositories.Payment;
    using Payments.Infrastructure.Database;
    using Payments.Infrastructure.Tests.Integration.Helpers;
    using Xunit;

    [Collection("DatabaseCollection")]
    public class PaymentRepositoryTests : IAsyncLifetime
    {
        private readonly DatabaseHelper<Guid, PaymentAggregateState> aggregateNameDatabaseHelper;

        private readonly PaymentRepository sut;

        public PaymentRepositoryTests(DatabaseFixture databaseFixture)
        {
            var connectionStringProvider = databaseFixture.ConnectionStringProvider;

            aggregateNameDatabaseHelper = new DatabaseHelper<Guid, PaymentAggregateState>("Payment", connectionStringProvider.PaymentsConnectionString, x => x.Id);

            var factory = new PaymentAggregateFactory();
            sut = new PaymentRepository(connectionStringProvider, factory);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await aggregateNameDatabaseHelper.CleanTableAsync();
        }

        [Theory(Skip = "TestContainers need updating")]
        [AutoData]
        public async void GetByIdAsync_GivenRecordDoesNotExist_ShouldReturnNewAggregate(Guid newId)
        {
            // Arrange
            // Act
            var result = await this.sut.GetByIdAsync(newId);

            // Assert
            result.IsNew.Should().BeTrue();
            result.UncommittedEvents.Should().BeEmpty();
        }

        [Theory(Skip = "TestContainers need updating")]
        [AutoData]
        public async void GetByIdAsync_GivenRecordExists_ShouldReturnAggregate(PaymentAggregateState state)
        {
            // Arrange
            await this.aggregateNameDatabaseHelper.AddRecordAsync(state);

            // Act
            var result = await this.sut.GetByIdAsync(state.Id);

            // Assert
            result.IsNew.Should().BeFalse();
            result.UncommittedEvents.Should().BeEmpty();
            result.Id.Should().Be(state.Id);
            result.State.Should().BeEquivalentTo(
                state,
                x => x
                    .Excluding(p => p.CreatedOn)
                    .Excluding(p => p.UpdatedOn));
            result.State.CreatedOn.Should().BeCloseTo(state.CreatedOn, TimeSpan.FromSeconds(1));
            result.State.UpdatedOn.Should().BeCloseTo(state.UpdatedOn, TimeSpan.FromSeconds(1));
        }

        [Theory(Skip = "TestContainers need updating")]
        [AutoData]
        public async void CreateAsync_GivenRecordDoesNotExist_CreatesRecord(PaymentAggregateState state)
        {
            // Arrange
            // Act
            await this.sut.CreateAsync(new PaymentAggregate(state));
            this.aggregateNameDatabaseHelper.TrackId(state.Id);

            var result = (await this.sut.GetByIdAsync(state.Id)).State;

            // Assert
            result.Id.Should().Be(state.Id);
            result.Should().BeEquivalentTo(
                state,
                x => x
                    .Excluding(p => p.CreatedOn)
                    .Excluding(p => p.UpdatedOn));
            result.CreatedOn.Should().BeCloseTo(state.CreatedOn, TimeSpan.FromSeconds(1));
            result.UpdatedOn.Should().BeCloseTo(state.UpdatedOn, TimeSpan.FromSeconds(1));
        }

        [Theory(Skip = "TestContainers need updating")]
        [AutoData]
        public async void UpdateAsync_GivenRecordExists_UpdatesRecord(PaymentAggregateState state, PaymentAggregateState updatedState)
        {
            //// non updateable properties
            updatedState.CreatedOn = state.CreatedOn;

            // Arrange
            await this.aggregateNameDatabaseHelper.AddRecordAsync(state);

            // Act
            updatedState.Id = state.Id;
            await this.sut.UpdateAsync(new PaymentAggregate(updatedState));

            var result = (await this.sut.GetByIdAsync(state.Id)).State;

            // Assert
            result.Id.Should().Be(state.Id);
            result.Should().BeEquivalentTo(
                updatedState,
                x => x
                    .Excluding(p => p.CreatedOn)
                    .Excluding(p => p.UpdatedOn));
            result.CreatedOn.Should().BeCloseTo(updatedState.CreatedOn, TimeSpan.FromSeconds(1));
            result.UpdatedOn.Should().BeCloseTo(updatedState.UpdatedOn, TimeSpan.FromSeconds(1));
        }
    }
}
