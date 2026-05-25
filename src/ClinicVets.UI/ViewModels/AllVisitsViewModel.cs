using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class AllVisitsViewModel : ViewModelBase
{
    private readonly IVisitService   _visits;
    private readonly IAnimalService  _animals;
    private readonly IEmployeeRepository _employees;

    [ObservableProperty] private string   _statusMsg  = string.Empty;
    [ObservableProperty] private VisitRow? _selected;
    [ObservableProperty] private string   _detailText = string.Empty;

    public ObservableCollection<VisitRow> Rows { get; } = new();

    public record VisitRow(Visit Visit, string AnimalName, string VetName);

    public AllVisitsViewModel(IVisitService visits, IAnimalService animals, IEmployeeRepository employees)
    {
        _visits    = visits;
        _animals   = animals;
        _employees = employees;
        Load();
    }

    private void Load()
    {
        Rows.Clear();
        var allAnimals = _animals.GetAll().ToDictionary(a => a.Id, a => a.Name);
        var allEmployees = _employees.GetAll().ToDictionary(e => e.Id, e => e.FullName);
        foreach (var v in _visits.GetAllVisits().OrderByDescending(v => v.VisitDateTime))
        {
            var name = allAnimals.TryGetValue(v.AnimalId, out var n) ? n : $"Animal #{v.AnimalId}";
            var vetName = allEmployees.TryGetValue(v.VetEmployeeId, out var vet) ? vet : $"Employee #{v.VetEmployeeId}";
            Rows.Add(new VisitRow(v, name, vetName));
        }
        StatusMsg = Rows.Count == 0 ? "No visits recorded." : $"{Rows.Count} visits total.";
    }

    [RelayCommand]
    private void Refresh() => Load();

    partial void OnSelectedChanged(VisitRow? value)
    {
        if (value is null) { DetailText = string.Empty; return; }
        var v    = value.Visit;
        var meds = v.Medicines.Count > 0
            ? string.Join("\n", v.Medicines.Select(m => $"  • {m.Name}  ₪{m.Price:F2}"))
            : "  (none)";
        DetailText =
            $"Visit #{v.Id}\n" +
            $"Animal   : {value.AnimalName}\n" +
            $"Date     : {v.VisitDateTime:dd/MM/yyyy HH:mm}\n" +
            $"Reason   : {v.Reason}\n" +
            $"Diagnosis: {(string.IsNullOrWhiteSpace(v.Diagnosis) ? "(none)" : v.Diagnosis)}\n" +
            $"Vet      : {value.VetName}\n" +
            $"Medicines:\n{meds}\n" +
            $"Cost     : ₪{v.TotalCost:F2}";
    }
}
