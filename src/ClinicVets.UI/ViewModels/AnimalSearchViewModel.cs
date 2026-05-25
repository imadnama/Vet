using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class AnimalSearchViewModel : ViewModelBase
{
    private readonly IAnimalService _animals;
    private List<Animal> _allAnimals = new();

    [ObservableProperty] private string  _nameQuery  = string.Empty;
    [ObservableProperty] private string  _chipQuery  = string.Empty;
    [ObservableProperty] private string  _errorMsg   = string.Empty;
    [ObservableProperty] private string  _typeFilter = "All";
    [ObservableProperty] private Animal? _selected;
    [ObservableProperty] private string  _detailText = string.Empty;

    public IReadOnlyList<string> TypeFilters { get; } = ["All", "Dog", "Cat", "Reptile", "Bird"];
    public ObservableCollection<Animal> Results { get; } = new();

    public AnimalSearchViewModel(IAnimalService animals)
    {
        _animals    = animals;
        _allAnimals = animals.GetAll().OrderBy(a => a.Name).ToList();
        foreach (var a in _allAnimals) Results.Add(a);
    }

    partial void OnTypeFilterChanged(string value) => ApplyFilter();

    [RelayCommand]
    private void Search() => ApplyFilter();

    private void ApplyFilter()
    {
        ErrorMsg   = string.Empty;
        Selected   = null;
        DetailText = string.Empty;
        Results.Clear();

        var filtered = _allAnimals.AsEnumerable();

        if (TypeFilter != "All" && Enum.TryParse<AnimalType>(TypeFilter, out var type))
            filtered = filtered.Where(a => a.Type == type);

        if (!string.IsNullOrWhiteSpace(ChipQuery))
        {
            var chip = ChipQuery.Trim();
            filtered = filtered.Where(a => a.ChipNumber == chip);
        }
        else if (!string.IsNullOrWhiteSpace(NameQuery))
        {
            var q = NameQuery.Trim();
            filtered = filtered.Where(a => a.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var a in filtered.OrderBy(a => a.Name))
            Results.Add(a);

        if (Results.Count == 0)
            ErrorMsg = "No animals match the current filter.";
    }

    partial void OnSelectedChanged(Animal? value)
    {
        if (value is null) { DetailText = string.Empty; return; }
        var vax   = value.LastVaccinationDate.HasValue
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
