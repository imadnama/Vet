namespace ClinicVets.Core.Models;

public class Visit
{
    public int Id { get; set; }

    /// <summary>FK to Animal.Id — required.</summary>
    public int AnimalId { get; set; }

    public string Reason { get; set; } = string.Empty;
    public DateTime VisitDateTime { get; set; }
    public string Diagnosis { get; set; } = string.Empty;

    /// <summary>FK to Employee.Id — must be a Veterinarian.</summary>
    public int VetEmployeeId { get; set; }

    /// <summary>Medicines administered during this visit.</summary>
    public List<Medicine> Medicines { get; set; } = new();

    /// <summary>Base visit price + sum of medicine prices.</summary>
    public decimal TotalCost { get; set; }
}
