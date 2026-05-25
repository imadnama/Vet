using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class AddAnimalViewModel : ViewModelBase
{
    private readonly IAnimalService      _animals;
    private readonly ICustomerRepository _customerRepo;
    private readonly Action              _onBack;

    [ObservableProperty] private string   _name              = string.Empty;
    [ObservableProperty] private AnimalType _selectedType    = AnimalType.Dog;
    [ObservableProperty] private decimal  _weight            = 1.0m;
    [ObservableProperty] private DateTimeOffset _birthDate   = DateTimeOffset.Now.AddYears(-1);
    [ObservableProperty] private Customer? _selectedOwner;
    [ObservableProperty] private DateTimeOffset? _lastVaccination;

    [ObservableProperty] private string _errName    = string.Empty;
    [ObservableProperty] private string _errWeight  = string.Empty;
    [ObservableProperty] private string _errBirth   = string.Empty;
    [ObservableProperty] private string _errOwner   = string.Empty;
    [ObservableProperty] private string _successMsg = string.Empty;
    [ObservableProperty] private string _errorMsg   = string.Empty;

    public ObservableCollection<Customer>  Owners     { get; } = new();
    public IEnumerable<AnimalType>         AnimalTypes => Enum.GetValues<AnimalType>();

    public AddAnimalViewModel(IAnimalService animals, ICustomerRepository customerRepo, Action onBack)
    {
        _animals      = animals;
        _customerRepo = customerRepo;
        _onBack       = onBack;

        foreach (var c in _customerRepo.GetAll())
            Owners.Add(c);
    }

    [RelayCommand]
    private void Save()
    {
        ErrName = ErrWeight = ErrBirth = ErrOwner = ErrorMsg = SuccessMsg = string.Empty;

        var animal = new Animal
        {
            Name                = Name.Trim(),
            Type                = SelectedType,
            Weight              = Weight,
            BirthDate           = BirthDate.DateTime,
            OwnerId             = SelectedOwner?.Id ?? 0,
            LastVaccinationDate = LastVaccination?.DateTime,
        };

        if (_animals.AddAnimal(animal, out var error))
            SuccessMsg = $"Animal '{animal.Name}' registered. Chip: {animal.ChipNumber}";
        else
            ErrorMsg = error;
    }

    [RelayCommand]
    private void Back() => _onBack();
}
