using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class CustomerSearchViewModel : ViewModelBase
{
    private readonly ICustomerService    _customers;
    private readonly IAnimalService      _animals;
    private readonly ICustomerRepository _customerRepo;
    private readonly List<Customer>      _allCustomers;

    [ObservableProperty] private string    _searchQuery      = string.Empty;
    [ObservableProperty] private bool      _searchByPhone    = false;
    [ObservableProperty] private string    _errorMessage     = string.Empty;
    [ObservableProperty] private Customer? _selectedCustomer;
    [ObservableProperty] private ObservableCollection<Animal> _customerAnimals = [];

    public ObservableCollection<Customer> Results { get; } = [];

    public CustomerSearchViewModel(ICustomerService customers, IAnimalService animals, ICustomerRepository customerRepo)
    {
        _customers    = customers;
        _animals      = animals;
        _customerRepo = customerRepo;
        _allCustomers = [.. customerRepo.GetAll().OrderBy(c => c.FullName)];
        foreach (var c in _allCustomers) Results.Add(c);
    }

    [RelayCommand]
    private void Search()
    {
        ErrorMessage     = string.Empty;
        SelectedCustomer = null;
        Results.Clear();

        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            foreach (var c in _allCustomers) Results.Add(c);
            return;
        }

        var q = SearchQuery.Trim();
        var filtered = SearchByPhone
            ? _allCustomers.Where(c => c.Phone.Contains(q, StringComparison.OrdinalIgnoreCase))
            : _allCustomers.Where(c => c.NationalId.Contains(q) ||
                                       c.FullName.Contains(q, StringComparison.OrdinalIgnoreCase));

        foreach (var c in filtered)
            Results.Add(c);

        if (Results.Count == 0)
            ErrorMessage = "No customers found.";
    }

    partial void OnSelectedCustomerChanged(Customer? value)
    {
        CustomerAnimals.Clear();
        if (value is null) return;
        foreach (var a in _customers.GetCustomerAnimals(value.Id))
            CustomerAnimals.Add(a);
    }
}
