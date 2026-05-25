using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Screen 4 — Search customers and view their linked animals.
/// Accessible only when CurrentUserSession.Role == Secretary.
///
/// TODO (g1):
///   - Add search controls: txtSearchId, txtSearchPhone, btnSearch.
///   - Show customer info in labels/textboxes (read-only).
///   - Show the customer's animals in a DataGridView by calling
///     _customerService.GetCustomerAnimals(customer.Id).
/// </summary>
public class CustomerSearchForm : Form
{
    private readonly ICustomerService _customerService;

    public CustomerSearchForm(ICustomerService customerService)
    {
        _customerService = customerService;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnSearch_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();
}
