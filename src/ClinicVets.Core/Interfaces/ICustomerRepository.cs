using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

public interface ICustomerRepository
{
    Customer? GetByNationalId(string nationalId);
    Customer? GetByPhone(string phone);
    bool NationalIdExists(string nationalId);
    void Add(Customer customer);
    IEnumerable<Customer> GetAll();
}
