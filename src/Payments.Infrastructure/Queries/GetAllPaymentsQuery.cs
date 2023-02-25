namespace Payments.Infrastructure.Queries
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using MySqlConnector;
    using Payments.Contracts.Responses;
    using Payments.Infrastructure.Database;

    public class GetAllPaymentsQuery : IGetAllPaymentsQuery
    {
        private readonly IConnectionStringProvider connectionStringProvider;
        private readonly SqlHelper<GetAllPaymentsQuery> sqlHelper;

        public GetAllPaymentsQuery(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.sqlHelper = new SqlHelper<GetAllPaymentsQuery>();
        }

        public async Task<IEnumerable<Payment>> ExecuteAsync()
        {
            await using var connection = new MySqlConnection(this.connectionStringProvider.PaymentsConnectionString);
            return await connection.QueryAsync<Payment>(
                this.sqlHelper.GetSqlFromEmbeddedResource("GetAllPayments"),
                commandType: CommandType.Text);
        }
    }
}