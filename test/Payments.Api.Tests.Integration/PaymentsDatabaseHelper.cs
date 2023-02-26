namespace Payments.Api.Tests.Integration
{
    using Dapper;
    using MySqlConnector;
    using Payments.Domain.Aggregates.Payment;
    using System;
    using System.Data;
    using System.Threading.Tasks;

    public class PaymentsDatabaseHelper : DatabaseHelper<Guid, PaymentAggregateState>
    {
        public PaymentsDatabaseHelper(string connectionString)
            : base("Payment", connectionString, x => x.Id)
        { }

        public async override Task AddRecordAsync(PaymentAggregateState record)
        {
            this.AddedRecords.Add(idSelector(record), record);

            var parameters = new
            {
                record.Id,
                record.MerchantId,
                record.CardHolderName,
                record.CardNumber,
                record.ExpiryMonth,
                record.ExpiryYear,
                record.Amount,
                record.CurrencyCode,
                record.Reference,
                Status = record.Status.ToString(),
                record.CreatedOn,
                record.UpdatedOn,
            };

            var query = @"
                INSERT INTO Payment (
	                Id,
	                MerchantId,
	                CardHolderName,
	                CardNumber,
	                ExpiryMonth,
	                ExpiryYear,
	                Amount,
	                CurrencyCode,
	                Reference,
	                Status,
	                CreatedOn,
	                UpdatedOn
                )
                VALUES (
	                @Id,
	                @MerchantId,
	                @CardHolderName,
	                @CardNumber,
	                @ExpiryMonth,
	                @ExpiryYear,
	                @Amount,
	                @CurrencyCode,
	                @Reference,
	                @Status,
	                @CreatedOn,
	                @UpdatedOn
                )";

            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);
        }
    }
}
