namespace Payments.Infrastructure.Database
{
    public class DatabaseOptions : IDatabaseOptions
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
