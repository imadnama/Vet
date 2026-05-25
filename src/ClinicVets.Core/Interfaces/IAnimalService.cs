using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

/// <summary>Implemented by g2 — AnimalService. All staff roles.</summary>
public interface IAnimalService
{
    bool AddAnimal(Animal animal, out string error);
    IEnumerable<Animal> GetAll();
    IEnumerable<Animal> SearchByName(string name);
    Animal? SearchByChipNumber(string chipNumber);

    /// <summary>True when more than 12 months have passed since LastVaccinationDate.</summary>
    bool NeedsVaccination(Animal animal);
}
