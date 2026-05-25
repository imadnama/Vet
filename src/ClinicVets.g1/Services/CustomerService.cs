using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Validation;

namespace ClinicVets.g1.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customers;
    private readonly IAnimalRepository   _animals;

    public CustomerService(ICustomerRepository customers, IAnimalRepository animals)
    {
        _customers = customers;
        _animals   = animals;
    }

    /// <summary>
    /// Validates then persists a new customer.
    /// Secretary role enforcement is handled by the calling form/navigation layer.
    /// </summary>
    public bool RegisterCustomer(Customer customer, out string error)
    {
        error = string.Empty;
        try
        {
            if (!CustomerValidator.ValidateFullName(customer.FullName, out error)) return false;
            if (!CustomerValidator.ValidateNationalId(customer.NationalId, out error)) return false;
            if (!CustomerValidator.ValidatePhone(customer.Phone, out error)) return false;
            if (!CustomerValidator.ValidateEmail(customer.Email, out error)) return false;

            if (_customers.NationalIdExists(customer.NationalId))
            {
                error = "A customer with this National ID already exists.";
                return false;
            }

            _customers.Add(customer);
            return true;
        }
        catch (Exception ex)
        {
            error = $"Unexpected error: {ex.Message}";
            return false;
        }
    }

    public Customer? SearchByNationalId(string nationalId)
        => _customers.GetByNationalId(nationalId);

    public Customer? SearchByPhone(string phone)
        => _customers.GetByPhone(phone);

    /// <summary>Returns all animals whose OwnerId matches the given customer.</summary>
    public IEnumerable<Animal> GetCustomerAnimals(int customerId)
        => _animals.GetByOwnerId(customerId);
}
