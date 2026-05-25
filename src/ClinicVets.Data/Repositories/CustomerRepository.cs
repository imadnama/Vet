using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using Microsoft.Data.Sqlite;

namespace ClinicVets.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DatabaseContext _db;

    public CustomerRepository(DatabaseContext db) => _db = db;

    public Customer? GetByNationalId(string nationalId)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, FullName, NationalId, Phone, Email " +
            "FROM Customers WHERE NationalId = @id";
        cmd.Parameters.AddWithValue("@id", nationalId);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapCustomer(reader) : null;
    }

    public Customer? GetByPhone(string phone)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, FullName, NationalId, Phone, Email " +
            "FROM Customers WHERE Phone = @p";
        cmd.Parameters.AddWithValue("@p", phone);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapCustomer(reader) : null;
    }

    public bool NationalIdExists(string nationalId)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM Customers WHERE NationalId = @id";
        cmd.Parameters.AddWithValue("@id", nationalId);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    public void Add(Customer customer)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Customers (FullName, NationalId, Phone, Email)
            VALUES (@fullName, @natId, @phone, @email)";
        cmd.Parameters.AddWithValue("@fullName", customer.FullName);
        cmd.Parameters.AddWithValue("@natId",    customer.NationalId);
        cmd.Parameters.AddWithValue("@phone",    customer.Phone);
        cmd.Parameters.AddWithValue("@email",    customer.Email);
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<Customer> GetAll()
    {
        var customers = new List<Customer>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, FullName, NationalId, Phone, Email
            FROM Customers
            ORDER BY FullName, NationalId;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            customers.Add(MapCustomer(reader));
        }
        return customers;
    }

    private static Customer MapCustomer(SqliteDataReader r) => new()
    {
        Id         = r.GetInt32(0),
        FullName   = r.GetString(1),
        NationalId = r.GetString(2),
        Phone      = r.GetString(3),
        Email      = r.GetString(4),
    };
}
