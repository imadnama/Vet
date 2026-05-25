using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customers;
    private readonly IAnimalRepository _animals;

    public CustomerService(ICustomerRepository customers, IAnimalRepository animals)
    {
        _customers = customers;
        _animals = animals;
    }

    /// <summary>Validates then persists a new customer. Secretary role only — enforce in the form.</summary>
    public bool RegisterCustomer(Customer customer, out string error)
        => throw new NotImplementedException();

    public Customer? SearchByNationalId(string nationalId)
        => throw new NotImplementedException();

    public Customer? SearchByPhone(string phone)
        => throw new NotImplementedException();

    /// <summary>Returns all animals whose OwnerId matches the given customer.</summary>
    public IEnumerable<Animal> GetCustomerAnimals(int customerId)
        => throw new NotImplementedException();
}
