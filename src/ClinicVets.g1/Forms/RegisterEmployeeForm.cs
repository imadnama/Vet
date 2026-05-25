using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Screen 2 — New employee registration.
///
/// Fields: Username, Password, ConfirmPassword, EmployeeNumber,
///         Email, NationalId, Role (RadioButton: Veterinarian / Secretary).
///
/// TODO (g1):
///   - Add all input controls and a Register button.
///   - On submit, call _authService.RegisterEmployee() after client-side
///     validation via EmployeeValidator for real-time inline feedback.
/// </summary>
public class RegisterEmployeeForm : Form
{
    private readonly IAuthService _authService;

    public RegisterEmployeeForm(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnRegister_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();
}
