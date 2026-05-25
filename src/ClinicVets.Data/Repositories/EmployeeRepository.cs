using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DatabaseContext _db;

    public EmployeeRepository(DatabaseContext db) => _db = db;

    public Employee? GetByUsername(string username) => throw new NotImplementedException();
    public bool UsernameExists(string username) => throw new NotImplementedException();
    public bool EmployeeNumberExists(string employeeNumber) => throw new NotImplementedException();
    public void Add(Employee employee) => throw new NotImplementedException();
    public IEnumerable<Employee> GetAll() => throw new NotImplementedException();
}
