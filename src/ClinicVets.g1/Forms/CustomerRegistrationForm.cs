using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Screen 3 — Register a new customer (animal owner).
/// Accessible only when CurrentUserSession.Role == Secretary.
///
/// Fields: FullName (letters only), NationalId (9 digits),
///         Phone, Email.
///
/// TODO (g1):
///   - Guard the constructor: redirect or show error if the current user is not Secretary.
///   - Validate each field with CustomerValidator before calling _customerService.RegisterCustomer().
/// </summary>
public class CustomerRegistrationForm : Form
{
    private readonly ICustomerService _customerService;

    public CustomerRegistrationForm(ICustomerService customerService)
    {
        _customerService = customerService;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnSave_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();
}
