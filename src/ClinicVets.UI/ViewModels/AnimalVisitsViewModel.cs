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

    [ObservableProperty] private string  _nameQuery      = string.Empty;
    [ObservableProperty] private string  _chipQuery      = string.Empty;
    [ObservableProperty] private string  _statusMsg      = string.Empty;
    [ObservableProperty] private Visit?  _selectedVisit;
    [ObservableProperty] private string  _detailText     = string.Empty;

    public ObservableCollection<Visit> Visits { get; } = new();

    public AnimalVisitsViewModel(IVisitService visits, IAnimalService animals)
    {
        _visits  = visits;
        _animals = animals;
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

    partial void OnSelectedVisitChanged(Visit? v)
    {
        if (v is null) { DetailText = string.Empty; return; }
        var meds = v.Medicines.Count > 0
            ? string.Join("\n", v.Medicines.Select(m => $"  • {m.Name}  ₪{m.Price:F2}"))
            : "  (none)";
        DetailText =
            $"Visit #{v.Id}\n" +
            $"Date     : {v.VisitDateTime:dd/MM/yyyy HH:mm}\n" +
            $"Reason   : {v.Reason}\n" +
            $"Diagnosis: {(string.IsNullOrWhiteSpace(v.Diagnosis) ? "(none)" : v.Diagnosis)}\n" +
            $"Vet ID   : {v.VetEmployeeId}\n" +
            $"Medicines:\n{meds}\n" +
            $"Cost     : ₪{v.TotalCost:F2}";
    }
}
