namespace Payments.Infrastructure.Database
{
    public interface IDatabaseOptions
    {
        string Server { get; set; }
        int? Port { get; set; }
        string Database { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}