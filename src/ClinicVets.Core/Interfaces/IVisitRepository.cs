using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

public interface IVisitRepository
{
    void Add(Visit visit);
    Visit? GetById(int id);
    IEnumerable<Visit> GetByAnimalId(int animalId);
    void Update(Visit visit);
}
