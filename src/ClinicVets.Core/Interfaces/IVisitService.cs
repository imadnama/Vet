using ClinicVets.Core.Models;

namespace ClinicVets.Core.Interfaces;

/// <summary>Implemented by g3 — VisitService. Veterinarian role only.</summary>
public interface IVisitService
{
    bool OpenVisit(Visit visit, out string error);
    IEnumerable<Visit> GetAllVisits();
    IEnumerable<Visit> GetVisitsByAnimal(int animalId);

    /// <summary>base + sum of all medicine prices for this visit.</summary>
    decimal CalculateTotalCost(decimal basePrice, IEnumerable<Medicine> medicines);
}
