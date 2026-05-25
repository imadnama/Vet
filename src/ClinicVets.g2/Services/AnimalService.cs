using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g2.Services;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animals;
    private readonly ICustomerRepository _customers;

    public AnimalService(IAnimalRepository animals, ICustomerRepository customers)
    {
        _animals = animals;
        _customers = customers;
    }

    /// <summary>
    /// Validates all fields (including owner existence) then generates a ChipNumber
    /// and persists the animal.
    /// </summary>
    public bool AddAnimal(Animal animal, out string error)
        => throw new NotImplementedException();

    public IEnumerable<Animal> SearchByName(string name)
        => throw new NotImplementedException();

    public Animal? SearchByChipNumber(string chipNumber)
        => throw new NotImplementedException();

    /// <summary>
    /// Returns true when LastVaccinationDate is null or more than 12 months ago
    /// relative to today — triggers the vaccination alert in g3.
    /// </summary>
    public bool NeedsVaccination(Animal animal)
        => throw new NotImplementedException();
}
