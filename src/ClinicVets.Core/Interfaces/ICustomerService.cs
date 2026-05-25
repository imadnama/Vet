using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

/// <summary>Implemented by g1 — CustomerService. Secretary role only.</summary>
public interface ICustomerService
{
    bool RegisterCustomer(Customer customer, out string error);
    Customer? SearchByNationalId(string nationalId);
    Customer? SearchByPhone(string phone);
    IEnumerable<Animal> GetCustomerAnimals(int customerId);
}
