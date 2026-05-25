using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

public interface IEmployeeRepository
{
    Employee? GetByUsername(string username);
    bool UsernameExists(string username);
    bool EmployeeNumberExists(string employeeNumber);
    void Add(Employee employee);
    IEnumerable<Employee> GetAll();
}
