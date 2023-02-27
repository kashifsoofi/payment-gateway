namespace Payments.Infrastructure.Database
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly DatabaseOptions databaseOptions;

        public ConnectionStringProvider(DatabaseOptions databaseOptions)
        {
            this.databaseOptions = databaseOptions;
        }

        public string PaymentsConnectionString
        {
            get
            {
                var port = databaseOptions.Port ?? 3306;
                var database = string.IsNullOrEmpty(databaseOptions.Database) ? "Payments" : databaseOptions.Database;
                var sslMode = databaseOptions.SslMode != null ? $"SSL Mode={databaseOptions.SslMode};" : "";
                return $"Server={databaseOptions.Server};Port={port};Database={database};Uid={databaseOptions.Username};Pwd={databaseOptions.Password};{sslMode}";
            }
        }
    }
}
