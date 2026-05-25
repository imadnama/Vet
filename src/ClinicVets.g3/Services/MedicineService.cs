using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g3.Services;

public class MedicineService : IMedicineService
{
    private readonly IMedicineRepository _medicines;

    public MedicineService(IMedicineRepository medicines) => _medicines = medicines;

    public bool AddMedicine(Medicine medicine, out string error)
        => throw new NotImplementedException();

    public bool DeleteMedicine(int id)
        => throw new NotImplementedException();

    public IEnumerable<Medicine> GetAll()
        => throw new NotImplementedException();
}
