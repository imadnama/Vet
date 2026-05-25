using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DatabaseContext _db;

    public CustomerRepository(DatabaseContext db) => _db = db;

    public Customer? GetByNationalId(string nationalId) => throw new NotImplementedException();
    public Customer? GetByPhone(string phone) => throw new NotImplementedException();
    public bool NationalIdExists(string nationalId) => throw new NotImplementedException();
    public void Add(Customer customer) => throw new NotImplementedException();
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
            customers.Add(new Customer
            {
                Id = reader.GetInt32(0),
                FullName = reader.GetString(1),
                NationalId = reader.GetString(2),
                Phone = reader.GetString(3),
                Email = reader.GetString(4)
            });
        }

        return customers;
    }
}
