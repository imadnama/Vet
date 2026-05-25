using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g3.Services;

public class VisitService : IVisitService
{
    private const decimal BaseVisitPrice = 100m; // adjust as needed

    private readonly IVisitRepository _visits;
    private readonly IAnimalRepository _animals;

    public VisitService(IVisitRepository visits, IAnimalRepository animals)
    {
        _visits = visits;
        _animals = animals;
    }

    /// <summary>
    /// Validates the visit (animal exists, role is vet), persists it, and
    /// auto-updates the animal's LastVaccinationDate if a vaccine was given.
    /// </summary>
    public bool OpenVisit(Visit visit, out string error)
        => throw new NotImplementedException();

    public IEnumerable<Visit> GetVisitsByAnimal(int animalId)
        => throw new NotImplementedException();

    /// <summary>base + sum of Medicine.Price for each medicine in the list.</summary>
    public decimal CalculateTotalCost(decimal basePrice, IEnumerable<Medicine> medicines)
        => throw new NotImplementedException();
}
