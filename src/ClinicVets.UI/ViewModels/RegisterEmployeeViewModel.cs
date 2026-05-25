using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Validation;

namespace ClinicVets.UI.ViewModels;

public partial class RegisterEmployeeViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly Action       _onBack;

    [ObservableProperty] private string _fullName        = string.Empty;
    [ObservableProperty] private string _username        = string.Empty;
    [ObservableProperty] private string _password        = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _employeeNumber  = string.Empty;
    [ObservableProperty] private string _email           = string.Empty;
    [ObservableProperty] private string _nationalId      = string.Empty;
    [ObservableProperty] private bool   _isVeterinarian  = true;

    [ObservableProperty] private string _errFullName        = string.Empty;
    [ObservableProperty] private string _errUsername        = string.Empty;
    [ObservableProperty] private string _errPassword        = string.Empty;
    [ObservableProperty] private string _errConfirmPassword = string.Empty;
    [ObservableProperty] private string _errEmployeeNumber  = string.Empty;
    [ObservableProperty] private string _errEmail           = string.Empty;
    [ObservableProperty] private string _errNationalId      = string.Empty;
    [ObservableProperty] private string _generalError       = string.Empty;
    [ObservableProperty] private string _successMessage     = string.Empty;

    public RegisterEmployeeViewModel(IAuthService auth, Action onBack)
    {
        _auth   = auth;
        _onBack = onBack;
    }

    [RelayCommand]
    private void Register()
    {
        ClearErrors();
        bool valid = true;

        if (string.IsNullOrWhiteSpace(FullName))   { ErrFullName = "Full name is required."; valid = false; }
        if (!EmployeeValidator.ValidateUsername(Username.Trim(), out var eu))       { ErrUsername = eu; valid = false; }
        if (!EmployeeValidator.ValidatePassword(Password, out var ep))              { ErrPassword = ep; valid = false; }
        else if (Password != ConfirmPassword)                                       { ErrConfirmPassword = "Passwords do not match."; valid = false; }
        if (!EmployeeValidator.ValidateEmployeeNumber(EmployeeNumber.Trim(), out var ee)) { ErrEmployeeNumber = ee; valid = false; }
        if (!EmployeeValidator.ValidateEmail(Email.Trim(), out var em))             { ErrEmail = em; valid = false; }
        if (!EmployeeValidator.ValidateNationalId(NationalId.Trim(), out var en))   { ErrNationalId = en; valid = false; }

        if (!valid) return;

        var emp = new Employee
        {
            FullName       = FullName.Trim(),
            Username       = Username.Trim(),
            EmployeeNumber = EmployeeNumber.Trim(),
            Email          = Email.Trim(),
            NationalId     = NationalId.Trim(),
            Role           = IsVeterinarian ? Role.Veterinarian : Role.Secretary,
        };

        if (_auth.RegisterEmployee(emp, Password))
        {
            SuccessMessage = $"Employee '{emp.Username}' registered successfully!";
            ClearForm();
        }
        else
        {
            GeneralError = "Username or Employee Number already exists.";
        }
    }

    [RelayCommand]
    private void Back() => _onBack();

    private void ClearErrors()
    {
        ErrFullName = ErrUsername = ErrPassword = ErrConfirmPassword = string.Empty;
        ErrEmployeeNumber = ErrEmail = ErrNationalId = GeneralError = SuccessMessage = string.Empty;
    }

    private void ClearForm()
    {
        FullName = Username = Password = ConfirmPassword = string.Empty;
        EmployeeNumber = Email = NationalId = string.Empty;
        IsVeterinarian = true;
    }
}
