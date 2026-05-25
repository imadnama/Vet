using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.Data.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly DatabaseContext _db;

    public AnimalRepository(DatabaseContext db) => _db = db;

    public Animal? GetByChipNumber(string chipNumber) => throw new NotImplementedException();
    public IEnumerable<Animal> SearchByName(string name) => throw new NotImplementedException();
    public IEnumerable<Animal> GetByOwnerId(int ownerId) => throw new NotImplementedException();
    public void Add(Animal animal) => throw new NotImplementedException();
    public void Update(Animal animal) => throw new NotImplementedException();
    public IEnumerable<Animal> GetAll() => throw new NotImplementedException();
}
