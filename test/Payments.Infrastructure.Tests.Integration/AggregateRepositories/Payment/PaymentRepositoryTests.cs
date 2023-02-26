namespace Payments.Infrastructure.Tests.Integration.AggregateRepositories.Payment
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using FluentAssertions;
    using Moq;
    using Payments.Domain.AcquiringBank;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.AggregateRepositories.Payment;
    using Payments.Infrastructure.Tests.Integration.Helpers;
    using Xunit;

    [Collection("DatabaseCollection")]
    public class PaymentRepositoryTests : IAsyncLifetime
    {
        private readonly Mock<IAcquiringBankService> acquiringBankServiceMock;

        private readonly PaymentsDatabaseHelper paymentsDatabaseHelper;

        private readonly PaymentRepository sut;

        public PaymentRepositoryTests(DatabaseFixture databaseFixture)
        {
            acquiringBankServiceMock = new Mock<IAcquiringBankService>();

            var connectionStringProvider = databaseFixture.ConnectionStringProvider;

            paymentsDatabaseHelper = new PaymentsDatabaseHelper(connectionStringProvider.PaymentsConnectionString);

            var factory = new PaymentAggregateFactory(acquiringBankServiceMock.Object);
            sut = new PaymentRepository(connectionStringProvider, factory);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await paymentsDatabaseHelper.CleanTableAsync();
        }

        [Theory]
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

        [Theory]
        [AutoData]
        public async void GetByIdAsync_GivenRecordExists_ShouldReturnAggregate(PaymentAggregateState state)
        {
            // Arrange
            state.CardNumber = state.CardNumber.Substring(0, 4);
            state.CurrencyCode = "GBP";
            await this.paymentsDatabaseHelper.AddRecordAsync(state);

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

        [Theory]
        [AutoData]
        public async void CreateAsync_GivenRecordDoesNotExist_CreatesRecord(PaymentAggregateState state)
        {
            // Arrange
            state.CardNumber = state.CardNumber.Substring(0, 4);
            state.CurrencyCode = "GBP";

            // Act
            await this.sut.CreateAsync(new PaymentAggregate(acquiringBankServiceMock.Object, state));
            this.paymentsDatabaseHelper.TrackId(state.Id);

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
    }
}
