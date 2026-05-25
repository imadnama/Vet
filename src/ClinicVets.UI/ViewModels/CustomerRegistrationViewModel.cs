using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Validation;

namespace ClinicVets.UI.ViewModels;

public partial class CustomerRegistrationViewModel : ViewModelBase
{
    private readonly ICustomerService _service;
    private readonly Action           _onBack;

    [ObservableProperty] private string _fullName      = string.Empty;
    [ObservableProperty] private string _nationalId    = string.Empty;
    [ObservableProperty] private string _phone         = string.Empty;
    [ObservableProperty] private string _email         = string.Empty;

    [ObservableProperty] private string _errFullName   = string.Empty;
    [ObservableProperty] private string _errNationalId = string.Empty;
    [ObservableProperty] private string _errPhone      = string.Empty;
    [ObservableProperty] private string _errEmail      = string.Empty;
    [ObservableProperty] private string _successMsg    = string.Empty;
    [ObservableProperty] private string _generalError  = string.Empty;

    public CustomerRegistrationViewModel(ICustomerService service, Action onBack)
    {
        _service = service;
        _onBack  = onBack;
    }

    [RelayCommand]
    private void Register()
    {
        ErrFullName = ErrNationalId = ErrPhone = ErrEmail = GeneralError = SuccessMsg = string.Empty;
        bool valid = true;

        if (!CustomerValidator.ValidateFullName(FullName.Trim(), out var ef))    { ErrFullName   = ef; valid = false; }
        if (!CustomerValidator.ValidateNationalId(NationalId.Trim(), out var en)){ ErrNationalId = en; valid = false; }
        if (!CustomerValidator.ValidatePhone(Phone.Trim(), out var ep))          { ErrPhone      = ep; valid = false; }
        if (!CustomerValidator.ValidateEmail(Email.Trim(), out var em))          { ErrEmail      = em; valid = false; }

        if (!valid) return;

        var customer = new Customer
        {
            FullName   = FullName.Trim(),
            NationalId = NationalId.Trim(),
            Phone      = Phone.Trim(),
            Email      = Email.Trim(),
        };

        if (_service.RegisterCustomer(customer, out var error))
        {
            SuccessMsg = $"Customer '{customer.FullName}' registered successfully!";
            FullName = NationalId = Phone = Email = string.Empty;
        }
        else
        {
            GeneralError = error;
        }
    }

    [RelayCommand]
    private void Back() => _onBack();
}
