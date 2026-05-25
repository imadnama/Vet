using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g2.Validation;

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
    {
        error = string.Empty;

        if (animal is null)
        {
            error = "Animal details are required.";
            return false;
        }

        animal.Name = animal.Name.Trim();

        if (!AnimalValidator.ValidateName(animal.Name, out error)
            || !AnimalValidator.ValidateWeight(animal.Weight, out error)
            || !AnimalValidator.ValidateBirthDate(animal.BirthDate, out error)
            || !AnimalValidator.ValidateOwnerId(animal.OwnerId, out error))
        {
            return false;
        }

        if (animal.LastVaccinationDate.HasValue
            && animal.LastVaccinationDate.Value.Date > DateTime.Today)
        {
            error = "Last vaccination date cannot be in the future.";
            return false;
        }

        if (!_customers.GetAll().Any(customer => customer.Id == animal.OwnerId))
        {
            error = "Selected owner does not exist.";
            return false;
        }

        animal.BirthDate = animal.BirthDate.Date;
        animal.LastVaccinationDate = animal.LastVaccinationDate?.Date;
        animal.ChipNumber = GenerateUniqueChipNumber();

        _animals.Add(animal);
        return true;
    }

    public IEnumerable<Animal> SearchByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Enumerable.Empty<Animal>();
        }

        return _animals.SearchByName(name.Trim());
    }

    public Animal? SearchByChipNumber(string chipNumber)
    {
        if (string.IsNullOrWhiteSpace(chipNumber))
        {
            return null;
        }

        return _animals.GetByChipNumber(chipNumber.Trim());
    }

    /// <summary>
    /// Returns true when LastVaccinationDate is null or more than 12 months ago
    /// relative to today. Used by g3 to trigger vaccination alerts.
    /// </summary>
    public bool NeedsVaccination(Animal animal)
    {
        if (animal.LastVaccinationDate is null)
        {
            return true;
        }

        return animal.LastVaccinationDate.Value.Date <= DateTime.Today.AddMonths(-12);
    }

    private string GenerateUniqueChipNumber()
    {
        string chipNumber;

        do
        {
            chipNumber = $"CV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 10000)}";
        }
        while (_animals.GetByChipNumber(chipNumber) is not null);

        return chipNumber;
    }
}
