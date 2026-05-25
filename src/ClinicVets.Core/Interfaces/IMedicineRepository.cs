using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

public interface IMedicineRepository
{
    void Add(Medicine medicine);
    void Delete(int id);
    Medicine? GetById(int id);
    IEnumerable<Medicine> GetAll();
    void Update(Medicine medicine);
}
