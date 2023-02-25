namespace Payments.Infrastructure.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Payments.Contracts.Responses;

    public interface IGetAllPaymentsQuery
    {
        Task<IEnumerable<Payment>> ExecuteAsync();
    }
}
