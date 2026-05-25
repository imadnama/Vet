using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class AnimalVisitsViewModel : ViewModelBase
{
    private readonly IVisitService  _visits;
    private readonly IAnimalService _animals;
    private readonly IEmployeeRepository _employees;

    [ObservableProperty] private string  _nameQuery      = string.Empty;
    [ObservableProperty] private string  _chipQuery      = string.Empty;
    [ObservableProperty] private string  _statusMsg      = string.Empty;
    [ObservableProperty] private Visit?  _selectedVisit;
    [ObservableProperty] private string  _detailText     = string.Empty;

    public ObservableCollection<Visit> Visits { get; } = new();

    public AnimalVisitsViewModel(IVisitService visits, IAnimalService animals, IEmployeeRepository employees)
    {
        _visits    = visits;
        _animals   = animals;
        _employees = employees;
    }

    [RelayCommand]
    private void Search()
    {
        StatusMsg = string.Empty;
        Visits.Clear();
        SelectedVisit = null;
        DetailText    = string.Empty;

        Animal? animal = null;
        if (!string.IsNullOrWhiteSpace(ChipQuery))
            animal = _animals.SearchByChipNumber(ChipQuery.Trim());
        else if (!string.IsNullOrWhiteSpace(NameQuery))
            animal = _animals.SearchByName(NameQuery.Trim()).FirstOrDefault();

        if (animal is null) { StatusMsg = "Animal not found."; return; }

        StatusMsg = $"Visits for: {animal.Name}  (Chip: {animal.ChipNumber})";
        foreach (var v in _visits.GetVisitsByAnimal(animal.Id).OrderByDescending(v => v.VisitDateTime))
            Visits.Add(v);

        if (Visits.Count == 0) StatusMsg += "  — No visits recorded.";
    }

    partial void OnSelectedVisitChanged(Visit? value)
    {
        if (value is null) { DetailText = string.Empty; return; }
        var meds = value.Medicines.Count > 0
            ? string.Join("\n", value.Medicines.Select(m => $"  • {m.Name}  ₪{m.Price:F2}"))
            : "  (none)";
        var vetName = _employees.GetAll().FirstOrDefault(e => e.Id == value.VetEmployeeId)?.FullName
            ?? $"Employee #{value.VetEmployeeId}";
        DetailText =
            $"Visit #{value.Id}\n" +
            $"Date     : {value.VisitDateTime:dd/MM/yyyy HH:mm}\n" +
            $"Reason   : {value.Reason}\n" +
            $"Diagnosis: {(string.IsNullOrWhiteSpace(value.Diagnosis) ? "(none)" : value.Diagnosis)}\n" +
            $"Vet      : {vetName}\n" +
            $"Medicines:\n{meds}\n" +
            $"Cost     : ₪{value.TotalCost:F2}";
    }
}
