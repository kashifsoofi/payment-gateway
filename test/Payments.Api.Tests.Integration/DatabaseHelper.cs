namespace Payments.Api.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using MySqlConnector;

    public class DatabaseHelper<TId, TRecord> where TRecord : class
    {
        private readonly string tableName;
        protected readonly string connectionString;
        protected readonly Func<TRecord, TId> idSelector;
        private readonly string idColumnName;

        public DatabaseHelper(
            string tableName,
            string connectionString,
            Func<TRecord, TId> idSelector,
            string idColumnName = "Id")
        {
            this.tableName = tableName;
            this.connectionString = connectionString;
            this.idSelector = idSelector;
            this.idColumnName = idColumnName;
        }

        public Dictionary<TId, TRecord> AddedRecords { get; } = new Dictionary<TId, TRecord>();

        public async Task<TRecord> GetRecordAsync(TId id)
        {
            await using var connection = new MySqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TRecord>(
                $"SELECT * FROM {tableName} WHERE {idColumnName} = @Id",
                new { Id = id },
                commandType: CommandType.Text);
        }

        public async virtual Task AddRecordAsync(TRecord record)
        {
            this.AddedRecords.Add(idSelector(record), record);
            await using var connection = new MySqlConnection(connectionString);
            await connection.InsertAsync<TRecord>(record);
        }

        public async Task AddRecordsAsync(IEnumerable<TRecord> records)
        {
            foreach (var record in records)
            {
                await AddRecordAsync(record);
            }
        }

        public void TrackId(TId id) => AddedRecords.Add(id, default);

        public async Task DeleteRecordAsync(TId id)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync(
                $"DELETE FROM {tableName} WHERE {idColumnName} = @Id",
                new { Id = id },
                commandType: CommandType.Text);
        }

        public async Task CleanTableAsync()
        {
            foreach (var addedRecord in AddedRecords)
            {
                await DeleteRecordAsync(addedRecord.Key);
            }
        }
    }
}
