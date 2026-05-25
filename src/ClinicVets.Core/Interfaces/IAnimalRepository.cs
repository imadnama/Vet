using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

public interface IAnimalRepository
{
    Animal? GetByChipNumber(string chipNumber);
    IEnumerable<Animal> SearchByName(string name);
    IEnumerable<Animal> GetByOwnerId(int ownerId);
    void Add(Animal animal);
    void Update(Animal animal);
    IEnumerable<Animal> GetAll();
}
