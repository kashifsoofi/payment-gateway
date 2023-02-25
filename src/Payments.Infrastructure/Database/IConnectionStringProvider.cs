using System;
namespace Payments.Infrastructure.Database
{
    public interface IConnectionStringProvider
    {
        string PaymentsConnectionString { get; }
    }
}
