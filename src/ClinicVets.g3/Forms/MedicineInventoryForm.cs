using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g3.Forms;

/// <summary>
/// Screen 9 — Medicine inventory management (add / delete medicines).
/// Accessible to all staff roles for viewing; add/delete restricted to Veterinarian.
///
/// TODO (g3 – Ammar Naameh):
///   - Show all medicines in a DataGridView.
///   - "Add" button opens an inline panel or sub-form for Name, Price, Quantity.
///   - "Delete" button removes the selected row after confirmation.
///   - Validate fields with MedicineValidator before calling _medicineService.AddMedicine().
/// </summary>
public class MedicineInventoryForm : Form
{
    private readonly IMedicineService _medicineService;

    public MedicineInventoryForm(IMedicineService medicineService)
    {
        _medicineService = medicineService;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnAdd_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();

    private void btnDelete_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();

    private void LoadMedicines()
        => throw new NotImplementedException();
}
