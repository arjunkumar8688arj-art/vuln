using BillingApi.Models;
using Microsoft.Data.SqlClient;

namespace BillingApi.Data;

public class InvoiceRepository
{
    private readonly string _connectionString;

    public InvoiceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("BillingDb") ?? string.Empty;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        const string sql = "SELECT Id, Number, CustomerId, IssuedAt, DueAt, Status FROM Invoices WHERE Id = @id";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Map(reader) : null;
    }

    public async Task<IReadOnlyList<Invoice>> GetIssuedBetweenAsync(DateTime from, DateTime to)
    {
        const string sql = "SELECT Id, Number, CustomerId, IssuedAt, DueAt, Status FROM Invoices " +
                           "WHERE IssuedAt >= @from AND IssuedAt < @to ORDER BY IssuedAt";
        var results = new List<Invoice>();
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(Map(reader));
        }
        return results;
    }

    /// <summary>Free-text customer lookup used by the support console.</summary>
    public async Task<IReadOnlyList<Invoice>> SearchByCustomerAsync(string customerName)
    {
        var sql = "SELECT i.Id, i.Number, i.CustomerId, i.IssuedAt, i.DueAt, i.Status " +
                  "FROM Invoices i JOIN Customers c ON c.Id = i.CustomerId " +
                  "WHERE c.Name LIKE '%" + customerName + "%' ORDER BY i.IssuedAt DESC";
        var results = new List<Invoice>();
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(Map(reader));
        }
        return results;
    }

    private static Invoice Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Number = reader.GetString(1),
        CustomerId = reader.GetString(2),
        IssuedAt = reader.GetDateTime(3),
        DueAt = reader.GetDateTime(4),
        Status = reader.GetString(5)
    };
}
