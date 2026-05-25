using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class CustomerSearchViewModel : ViewModelBase
{
    private readonly ICustomerService _customers;
    private readonly IAnimalService   _animals;

    [ObservableProperty] private string   _searchQuery   = string.Empty;
    [ObservableProperty] private bool     _searchByPhone = false;
    [ObservableProperty] private string   _errorMessage  = string.Empty;
    [ObservableProperty] private Customer? _foundCustomer;
    [ObservableProperty] private ObservableCollection<Animal> _customerAnimals = new();

    public CustomerSearchViewModel(ICustomerService customers, IAnimalService animals)
    {
        _customers = customers;
        _animals   = animals;
    }

    [RelayCommand]
    private void Search()
    {
        ErrorMessage  = string.Empty;
        FoundCustomer = null;
        CustomerAnimals.Clear();

        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            ErrorMessage = "Enter a National ID or phone number to search.";
            return;
        }

        var q = SearchQuery.Trim();
        var result = SearchByPhone
            ? _customers.SearchByPhone(q)
            : _customers.SearchByNationalId(q);

        if (result is null)
        {
            ErrorMessage = "No customer found.";
            return;
        }

        FoundCustomer = result;
        foreach (var a in _customers.GetCustomerAnimals(result.Id))
            CustomerAnimals.Add(a);
    }
}
