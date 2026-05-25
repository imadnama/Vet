using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.Data.Repositories;

public class MedicineRepository : IMedicineRepository
{
    private readonly DatabaseContext _db;

    public MedicineRepository(DatabaseContext db) => _db = db;

    public void Add(Medicine medicine) => throw new NotImplementedException();
    public void Delete(int id) => throw new NotImplementedException();
    public Medicine? GetById(int id) => throw new NotImplementedException();
    public IEnumerable<Medicine> GetAll() => throw new NotImplementedException();
    public void Update(Medicine medicine) => throw new NotImplementedException();
}
