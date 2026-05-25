using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g3.Validation;

namespace ClinicVets.g3.Services;

public class VisitService : IVisitService
{
    private const decimal BaseVisitPrice = 100m;

    private readonly IVisitRepository    _visits;
    private readonly IAnimalRepository   _animals;
    private readonly IMedicineRepository _medicines;

    public VisitService(
        IVisitRepository    visits,
        IAnimalRepository   animals,
        IMedicineRepository medicines)
    {
        _visits    = visits;
        _animals   = animals;
        _medicines = medicines;
    }

    /// <summary>
    /// Validates the visit, persists it, and returns the calculated cost.
    /// </summary>
    public bool OpenVisit(Visit visit, out string error)
    {
        // Validate inputs
        if (!VisitValidator.ValidateReason(visit.Reason, out var reasonError))
        {
            error = reasonError;
            return false;
        }

        if (!VisitValidator.ValidateDateTime(visit.VisitDateTime, out var dateError))
        {
            error = dateError;
            return false;
        }

        if (!VisitValidator.ValidateAnimalId(visit.AnimalId, out var animalError))
        {
            error = animalError;
            return false;
        }

        if (!VisitValidator.ValidateDiagnosis(visit.Diagnosis, out var diagError))
        {
            error = diagError;
            return false;
        }

        if (!_animals.GetAll().Any(a => a.Id == visit.AnimalId))
        {
            error = "Selected animal not found in database.";
            return false;
        }

        // Calculate total cost
        visit.TotalCost = CalculateTotalCost(BaseVisitPrice, visit.Medicines);

        // Persist the visit
        try
        {
            _visits.Add(visit);

            // Decrement stock for each medicine used
            foreach (var med in visit.Medicines)
            {
                var current = _medicines.GetById(med.Id);
                if (current is not null && current.Quantity > 0)
                {
                    current.Quantity--;
                    _medicines.Update(current);
                }
            }

            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"Failed to save visit: {ex.Message}";
            return false;
        }
    }

    public IEnumerable<Visit> GetAllVisits() => _visits.GetAll();

    public IEnumerable<Visit> GetVisitsByAnimal(int animalId) =>
        _visits.GetByAnimalId(animalId);

    /// <summary>
    /// Base price + sum of all medicine prices.
    /// </summary>
    public decimal CalculateTotalCost(decimal basePrice, IEnumerable<Medicine> medicines)
    {
        var medicineSum = medicines.Sum(m => m.Price);
        return basePrice + medicineSum;
    }
}
