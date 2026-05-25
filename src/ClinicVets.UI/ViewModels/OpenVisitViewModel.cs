using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.Core.Session;

namespace ClinicVets.UI.ViewModels;

public partial class OpenVisitViewModel : ViewModelBase
{
    private readonly IVisitService    _visits;
    private readonly IAnimalService   _animals;
    private readonly IMedicineService _medicines;
    private readonly Action           _onBack;

    [ObservableProperty] private Animal?         _selectedAnimal;
    [ObservableProperty] private string          _reason           = string.Empty;
    [ObservableProperty] private DateTimeOffset  _visitDate        = DateTimeOffset.Now;
    [ObservableProperty] private string          _diagnosis        = string.Empty;
    [ObservableProperty] private string          _vetName          = string.Empty;
    [ObservableProperty] private decimal         _totalCost        = 100m;
    [ObservableProperty] private string          _errorMsg         = string.Empty;
    [ObservableProperty] private string          _successMsg       = string.Empty;
    [ObservableProperty] private string          _vaccineWarning   = string.Empty;

    public ObservableCollection<Animal>   Animals   { get; } = new();
    public ObservableCollection<Medicine> Medicines { get; } = new();
    public ObservableCollection<Medicine> Selected  { get; } = new();

    public OpenVisitViewModel(IVisitService visits, IAnimalService animals, IMedicineService medicines, Action onBack)
    {
        _visits    = visits;
        _animals   = animals;
        _medicines = medicines;
        _onBack    = onBack;
        VetName    = CurrentUserSession.CurrentUser?.FullName ?? string.Empty;

        foreach (var a in _animals.GetAll())   Animals.Add(a);
        foreach (var m in _medicines.GetAll()) Medicines.Add(m);
    }

    partial void OnSelectedAnimalChanged(Animal? value)
    {
        VaccineWarning = (value is not null && _animals.NeedsVaccination(value))
            ? "⚠ This animal is due for its annual vaccination!"
            : string.Empty;
    }

    public void ToggleMedicine(Medicine m, bool add)
    {
        if (add && !Selected.Contains(m)) Selected.Add(m);
        else if (!add) Selected.Remove(m);
        TotalCost = _visits.CalculateTotalCost(100m, Selected);
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
            VisitDateTime = VisitDate.DateTime,
            Diagnosis     = Diagnosis,
            VetEmployeeId = CurrentUserSession.CurrentUser?.Id ?? 0,
            Medicines     = Selected.ToList(),
        };

        if (_visits.OpenVisit(visit, out var error))
        {
            SuccessMsg = $"Visit saved. Total: ₪{visit.TotalCost:F2}";
            Reason = Diagnosis = string.Empty;
            SelectedAnimal = null;
            Selected.Clear();
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
