namespace Payments.Infrastructure.AggregateRepositories.Payment
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using MySqlConnector;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.Database;

    public class PaymentRepository : IPaymentAggregateReadRepository, IPaymentAggregateWriteRepository
    {
        private readonly IConnectionStringProvider connectionStringProvider;
        private readonly IPaymentAggregateFactory aggregateFactory;
        private readonly SqlHelper<PaymentRepository> sqlHelper;

        public PaymentRepository(IConnectionStringProvider connectionStringProvider, IPaymentAggregateFactory aggregateFactory)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.aggregateFactory = aggregateFactory;
            this.sqlHelper = new SqlHelper<PaymentRepository>();
        }

        public async Task<IPaymentAggregate> GetByIdAsync(Guid id)
        {
            await using var connection = new MySqlConnection(this.connectionStringProvider.PaymentsConnectionString);
            {
                var state = await connection.QueryFirstOrDefaultAsync<PaymentAggregateState>(
                    this.sqlHelper.GetSqlFromEmbeddedResource("GetPaymentById"),
                    new { Id = id },
                    commandType: CommandType.Text);

                return state == null ? aggregateFactory.Create(id) : aggregateFactory.Create(state);
            }
        }

        public async Task CreateAsync(IPaymentAggregate aggregate)
        {
            await using var connection = new MySqlConnection(this.connectionStringProvider.PaymentsConnectionString);
            {
                var parameters = new
                {
                    aggregate.Id,
                    aggregate.State.CreatedOn,
                    aggregate.State.UpdatedOn,
                };

                await connection.ExecuteAsync(this.sqlHelper.GetSqlFromEmbeddedResource("CreatePayment"), parameters,
                    commandType: CommandType.Text);
            }
        }

        public async Task UpdateAsync(IPaymentAggregate aggregate)
        {
            await using var connection = new MySqlConnection(this.connectionStringProvider.PaymentsConnectionString);
            {
                var parameters = new
                {
                    aggregate.Id,
                    aggregate.State.UpdatedOn,
                };

                await connection.ExecuteAsync(this.sqlHelper.GetSqlFromEmbeddedResource("UpdatePayment"), parameters,
                    commandType: CommandType.Text);
            }
        }

        public async Task DeleteAsync(IPaymentAggregate aggregate)
        {
            await using var connection = new MySqlConnection(this.connectionStringProvider.PaymentsConnectionString);
            {
                var parameters = new
                {
                    aggregate.Id,
                };

                await connection.ExecuteAsync(this.sqlHelper.GetSqlFromEmbeddedResource("DeletePayment"), parameters,
                    commandType: CommandType.Text);
            }
        }
    }
}
