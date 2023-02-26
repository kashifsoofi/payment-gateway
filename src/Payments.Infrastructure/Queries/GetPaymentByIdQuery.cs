namespace Payments.Infrastructure.Queries
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using MySqlConnector;
    using Payments.Contracts.Responses;
    using Payments.Infrastructure.Database;

    public class GetPaymentByIdQuery : IGetPaymentByIdQuery
    {
        private readonly IConnectionStringProvider connectionStringProvider;
        private readonly SqlHelper<GetPaymentByIdQuery> sqlHelper;

        public GetPaymentByIdQuery(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.sqlHelper = new SqlHelper<GetPaymentByIdQuery>();
        }

        public async Task<Payment> ExecuteAsync(Guid id, Guid merchantId)
        {
            await using var connection = new MySqlConnection(this.connectionStringProvider.PaymentsConnectionString);
            return await connection.QueryFirstOrDefaultAsync<Payment>(
                this.sqlHelper.GetSqlFromEmbeddedResource("GetPaymentById"),
                new { Id = id, MerchantId = merchantId },
                commandType: CommandType.Text);
        }
    }
}