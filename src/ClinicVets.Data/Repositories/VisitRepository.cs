using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.Data.Repositories;

public class VisitRepository : IVisitRepository
{
    private readonly DatabaseContext _db;

    public VisitRepository(DatabaseContext db) => _db = db;

    public void Add(Visit visit) => throw new NotImplementedException();
    public Visit? GetById(int id) => throw new NotImplementedException();
    public IEnumerable<Visit> GetByAnimalId(int animalId) => throw new NotImplementedException();
    public void Update(Visit visit) => throw new NotImplementedException();
}
