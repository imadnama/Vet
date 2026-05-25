using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class AnimalSearchViewModel : ViewModelBase
{
    private readonly IAnimalService _animals;

    [ObservableProperty] private string  _nameQuery  = string.Empty;
    [ObservableProperty] private string  _chipQuery  = string.Empty;
    [ObservableProperty] private string  _errorMsg   = string.Empty;
    [ObservableProperty] private Animal? _selected;
    [ObservableProperty] private string  _detailText = string.Empty;

    public ObservableCollection<Animal> Results { get; } = new();

    public AnimalSearchViewModel(IAnimalService animals) => _animals = animals;

    [RelayCommand]
    private void Search()
    {
        ErrorMsg = string.Empty;
        Results.Clear();
        Selected   = null;
        DetailText = string.Empty;

        if (!string.IsNullOrWhiteSpace(ChipQuery))
        {
            var a = _animals.SearchByChipNumber(ChipQuery.Trim());
            if (a is not null) Results.Add(a);
            else ErrorMsg = "No animal found with that chip number.";
            return;
        }

        if (!string.IsNullOrWhiteSpace(NameQuery))
        {
            foreach (var a in _animals.SearchByName(NameQuery.Trim()))
                Results.Add(a);
            if (Results.Count == 0) ErrorMsg = "No animals found.";
            return;
        }

        ErrorMsg = "Enter a name or chip number.";
    }

    partial void OnSelectedChanged(Animal? value)
    {
        if (value is null) { DetailText = string.Empty; return; }
        var vax = value.LastVaccinationDate.HasValue
            ? value.LastVaccinationDate.Value.ToString("dd/MM/yyyy")
            : "None recorded";
        var needs = _animals.NeedsVaccination(value) ? "  ⚠ Vaccination overdue!" : string.Empty;
        DetailText =
            $"Name        : {value.Name}\n" +
            $"Chip        : {value.ChipNumber}\n" +
            $"Type        : {value.Type}\n" +
            $"Weight      : {value.Weight} kg\n" +
            $"Born        : {value.BirthDate:dd/MM/yyyy}\n" +
            $"Vaccination : {vax}{needs}";
    }
}
