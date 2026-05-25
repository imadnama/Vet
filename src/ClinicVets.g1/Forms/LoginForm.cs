using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Screen 1 — Staff login.
/// On success, stores the authenticated Employee in CurrentUserSession and
/// opens MainForm. On failure, shows an inline error label.
///
/// TODO (g1):
///   - Add controls: lblTitle, txtUsername, txtPassword (PasswordChar='*'),
///     btnLogin, lblError.
///   - Implement InitializeComponent() to lay out all controls.
///   - Implement btnLogin_Click to call _authService.Login() and navigate.
/// </summary>
public class LoginForm : Form
{
    private readonly IAuthService _authService;

    // Called by Program.cs after a successful login so LoginForm doesn't need
    // to know the concrete type of MainForm (avoids a circular project reference).
    private readonly Action _onLoginSuccess;

    public LoginForm(IAuthService authService, Action onLoginSuccess)
    {
        _authService = authService;
        _onLoginSuccess = onLoginSuccess;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnLogin_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();

    private void lnkRegister_LinkClicked(object? sender, EventArgs e)
        => throw new NotImplementedException();
}
