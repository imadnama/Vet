using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

/// <summary>Implemented by g3 — MedicineService.</summary>
public interface IMedicineService
{
    bool AddMedicine(Medicine medicine, out string error);
    bool DeleteMedicine(int id);
    IEnumerable<Medicine> GetAll();
}
