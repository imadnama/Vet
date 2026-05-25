using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g3.Validation;

namespace ClinicVets.g3.Services;

public class MedicineService : IMedicineService
{
    private readonly IMedicineRepository _medicines;

    public MedicineService(IMedicineRepository medicines) => _medicines = medicines;

    public bool AddMedicine(Medicine medicine, out string error)
    {
        if (!MedicineValidator.ValidateName(medicine.Name, out var nameError))
        {
            error = nameError;
            return false;
        }

        if (!MedicineValidator.ValidatePrice(medicine.Price, out var priceError))
        {
            error = priceError;
            return false;
        }

        if (!MedicineValidator.ValidateQuantity(medicine.Quantity, out var qtyError))
        {
            error = qtyError;
            return false;
        }

        try
        {
            _medicines.Add(medicine);
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"Failed to add medicine: {ex.Message}";
            return false;
        }
    }

    public bool DeleteMedicine(int id)
    {
        try
        {
            _medicines.Delete(id);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerable<Medicine> GetAll() => _medicines.GetAll();
}
