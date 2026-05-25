using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Session;

namespace ClinicVets.UI.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly Action       _onSuccess;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _error    = string.Empty;
    [ObservableProperty] private bool   _isBusy   = false;

    public LoginViewModel(IAuthService auth, Action onSuccess)
    {
        _auth      = auth;
        _onSuccess = onSuccess;
    }

    [RelayCommand]
    private void Login()
    {
        Error = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            Error = "Username and password are required.";
            return;
        }

        if (!_auth.Login(Username.Trim(), Password, out var employee))
        {
            Error = "Invalid username or password.";
            return;
        }

        CurrentUserSession.SetUser(employee!);
        _onSuccess();
    }
}
