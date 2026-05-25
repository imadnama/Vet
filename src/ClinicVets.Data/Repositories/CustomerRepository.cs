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
    public IEnumerable<Customer> GetAll() => throw new NotImplementedException();
}
