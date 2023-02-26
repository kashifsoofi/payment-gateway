namespace Payments.Infrastructure.Queries
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using MySqlConnector;
    using Payments.Contracts.Responses;
    using Payments.Infrastructure.Database;

    public class GetPaymentsByMerchantIdQuery : IGetPaymentsByMerchantIdQuery
    {
        private readonly IConnectionStringProvider connectionStringProvider;
        private readonly SqlHelper<GetPaymentsByMerchantIdQuery> sqlHelper;

        public GetPaymentsByMerchantIdQuery(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.sqlHelper = new SqlHelper<GetPaymentsByMerchantIdQuery>();
        }

        public async Task<IEnumerable<Payment>> ExecuteAsync(Guid merchantId)
        {
            await using var connection = new MySqlConnection(this.connectionStringProvider.PaymentsConnectionString);
            return await connection.QueryAsync<Payment>(
                this.sqlHelper.GetSqlFromEmbeddedResource("GetPaymentsByMerchantId"),
                new { MerchantId = merchantId },
                commandType: CommandType.Text);
        }
    }
}