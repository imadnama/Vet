using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Session;

namespace ClinicVets.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthService     _auth;
    private readonly ICustomerService _customers;
    private readonly IAnimalService   _animals;
    private readonly IVisitService    _visits;
    private readonly IMedicineService _medicines;
    private readonly ICustomerRepository _customerRepo;
    private readonly IEmployeeRepository _employeeRepo;

    [ObservableProperty] private ViewModelBase _currentPage = null!;
    [ObservableProperty] private bool          _isLoggedIn  = false;
    [ObservableProperty] private string        _welcomeText = string.Empty;
    [ObservableProperty] private bool          _isVet       = false;
    [ObservableProperty] private bool          _isSecretary = false;

    public MainViewModel(
        IAuthService auth, ICustomerService customers, IAnimalService animals,
        IVisitService visits, IMedicineService medicines, ICustomerRepository customerRepo, IEmployeeRepository employeeRepo)
    {
        _auth         = auth;
        _customers    = customers;
        _animals      = animals;
        _visits       = visits;
        _medicines    = medicines;
        _customerRepo = customerRepo;
        _employeeRepo = employeeRepo;

        CurrentPage = new LoginViewModel(_auth, OnLoginSuccess, NavigateToRegisterFromLogin);
    }

    private void OnLoginSuccess()
    {
        var user     = CurrentUserSession.CurrentUser!;
        IsLoggedIn   = true;
        IsVet        = CurrentUserSession.IsVeterinarian;
        IsSecretary  = CurrentUserSession.IsSecretary;
        WelcomeText  = $"Welcome, {user.FullName}";
        NavigateToDashboard();
    }

    [RelayCommand]
    public void NavigateToDashboard()
        => CurrentPage = new DashboardViewModel(
                CurrentUserSession.CurrentUser!,
                _visits, _animals, _medicines, _customerRepo);

    private void NavigateToRegisterFromLogin()
        => CurrentPage = new RegisterEmployeeViewModel(_auth, GoBackToLogin);

    private void GoBackToLogin()
        => CurrentPage = new LoginViewModel(_auth, OnLoginSuccess, NavigateToRegisterFromLogin);

    [RelayCommand]
    public void NavigateToRegisterEmployee()
        => CurrentPage = new RegisterEmployeeViewModel(_auth, NavigateToDashboard);

    [RelayCommand]
    public void NavigateToCustomerRegistration()
    {
        if (!CurrentUserSession.IsSecretary) return;
        CurrentPage = new CustomerRegistrationViewModel(_customers, NavigateToDashboard);
    }

    [RelayCommand]
    public void NavigateToCustomerSearch()
    {
        if (!CurrentUserSession.IsSecretary) return;
        CurrentPage = new CustomerSearchViewModel(_customers, _animals, _customerRepo);
    }

    [RelayCommand]
    public void NavigateToAddAnimal()
        => CurrentPage = new AddAnimalViewModel(_animals, _customerRepo, NavigateToDashboard);

    [RelayCommand]
    public void NavigateToAnimalSearch()
        => CurrentPage = new AnimalSearchViewModel(_animals);

    [RelayCommand]
    public void NavigateToOpenVisit()
    {
        if (!CurrentUserSession.IsVeterinarian) return;
        CurrentPage = new OpenVisitViewModel(_visits, _animals, _medicines, NavigateToDashboard);
    }

    [RelayCommand]
    public void NavigateToMedicineInventory()
    {
        if (!CurrentUserSession.IsVeterinarian) return;
        CurrentPage = new MedicineInventoryViewModel(_medicines);
    }

    [RelayCommand]
    public void NavigateToAnimalVisits()
    {
        if (!CurrentUserSession.IsVeterinarian) return;
        CurrentPage = new AnimalVisitsViewModel(_visits, _animals, _employeeRepo);
    }

    [RelayCommand]
    public void NavigateToAllVisits()
    {
        if (!CurrentUserSession.IsVeterinarian) return;
        CurrentPage = new AllVisitsViewModel(_visits, _animals, _employeeRepo);
    }

    [RelayCommand]
    public void Logout()
    {
        CurrentUserSession.Clear();
        IsLoggedIn  = false;
        IsVet       = false;
        IsSecretary = false;
        WelcomeText = string.Empty;
        CurrentPage = new LoginViewModel(_auth, OnLoginSuccess, NavigateToRegisterFromLogin);
    }
}
