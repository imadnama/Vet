using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class MedicineInventoryViewModel : ViewModelBase
{
    private readonly IMedicineService _service;

    [ObservableProperty] private string    _name       = string.Empty;
    [ObservableProperty] private decimal   _price      = 0.01m;
    [ObservableProperty] private int       _quantity   = 0;
    [ObservableProperty] private string    _errName    = string.Empty;
    [ObservableProperty] private string    _errPrice   = string.Empty;
    [ObservableProperty] private string    _errQty     = string.Empty;
    [ObservableProperty] private string    _successMsg = string.Empty;
    [ObservableProperty] private string    _errorMsg   = string.Empty;
    [ObservableProperty] private Medicine? _selected;

    public ObservableCollection<Medicine> Medicines { get; } = new();

    public MedicineInventoryViewModel(IMedicineService service)
    {
        _service = service;
        Reload();
    }

    private void Reload()
    {
        Medicines.Clear();
        foreach (var m in _service.GetAll())
            Medicines.Add(m);
    }

    [RelayCommand]
    private void Add()
    {
        ErrName = ErrPrice = ErrQty = ErrorMsg = SuccessMsg = string.Empty;

        var m = new Medicine { Name = Name.Trim(), Price = Price, Quantity = Quantity };
        if (_service.AddMedicine(m, out var error))
        {
            SuccessMsg = $"'{m.Name}' added.";
            Name = string.Empty; Price = 0.01m; Quantity = 0;
            Reload();
        }
        else ErrorMsg = error;
    }

    [RelayCommand]
    private void Delete()
    {
        if (Selected is null) { ErrorMsg = "Select a medicine to delete."; return; }
        if (_service.DeleteMedicine(Selected.Id))
        {
            SuccessMsg = "Medicine deleted.";
            Reload();
        }
        else ErrorMsg = "Failed to delete.";
    }
}
