using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.Core.Session;

namespace ClinicVets.UI.ViewModels;

public partial class MedicineItem : ObservableObject
{
    public Medicine Medicine { get; }

    [ObservableProperty] private bool _isSelected;

    public MedicineItem(Medicine medicine) => Medicine = medicine;
}

public partial class OpenVisitViewModel : ViewModelBase
{
    private readonly IVisitService    _visits;
    private readonly IAnimalService   _animals;
    private readonly IMedicineService _medicines;
    private readonly Action           _onBack;

    [ObservableProperty] private Animal?         _selectedAnimal;
    [ObservableProperty] private string          _reason           = string.Empty;
    [ObservableProperty] private DateTimeOffset  _visitDate        = DateTimeOffset.Now;
    [ObservableProperty] private TimeSpan        _visitTime        = DateTimeOffset.Now.TimeOfDay;
    [ObservableProperty] private string          _diagnosis        = string.Empty;
    [ObservableProperty] private string          _vetName          = string.Empty;
    [ObservableProperty] private decimal         _totalCost        = 100m;
    [ObservableProperty] private string          _errorMsg         = string.Empty;
    [ObservableProperty] private string          _successMsg       = string.Empty;
    [ObservableProperty] private string          _vaccineWarning   = string.Empty;

    public ObservableCollection<Animal>       Animals       { get; } = new();
    public ObservableCollection<MedicineItem> MedicineItems { get; } = new();

    public OpenVisitViewModel(IVisitService visits, IAnimalService animals, IMedicineService medicines, Action onBack)
    {
        _visits    = visits;
        _animals   = animals;
        _medicines = medicines;
        _onBack    = onBack;
        VetName    = CurrentUserSession.CurrentUser?.FullName ?? string.Empty;

        foreach (var a in _animals.GetAll())
            Animals.Add(a);

        foreach (var m in _medicines.GetAll().Where(m => m.Quantity > 0))
        {
            var item = new MedicineItem(m);
            item.PropertyChanged += (_, _) => RecalculateCost();
            MedicineItems.Add(item);
        }
    }

    private void RecalculateCost()
    {
        var selected = MedicineItems.Where(i => i.IsSelected).Select(i => i.Medicine);
        TotalCost = _visits.CalculateTotalCost(100m, selected);
    }

    partial void OnSelectedAnimalChanged(Animal? value)
    {
        VaccineWarning = (value is not null && _animals.NeedsVaccination(value))
            ? "⚠ This animal is due for its annual vaccination!"
            : string.Empty;
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMsg = SuccessMsg = string.Empty;

        if (SelectedAnimal is null) { ErrorMsg = "Please select an animal."; return; }

        var visit = new Visit
        {
            AnimalId      = SelectedAnimal.Id,
            Reason        = Reason,
            VisitDateTime = VisitDate.Date + VisitTime,
            Diagnosis     = Diagnosis,
            VetEmployeeId = CurrentUserSession.CurrentUser?.Id ?? 0,
            Medicines     = MedicineItems.Where(i => i.IsSelected).Select(i => i.Medicine).ToList(),
        };

        if (_visits.OpenVisit(visit, out var error))
        {
            SuccessMsg = $"Visit saved. Total: ₪{visit.TotalCost:F2}";
            Reason = Diagnosis = string.Empty;
            SelectedAnimal = null;
            foreach (var item in MedicineItems) item.IsSelected = false;
            TotalCost = 100m;
        }
        else
        {
            ErrorMsg = error;
        }
    }

    [RelayCommand]
    private void Back() => _onBack();
}
