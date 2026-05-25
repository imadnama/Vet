using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.Core.Session;
using ClinicVets.g3.Validation;

namespace ClinicVets.g3.Forms;

public class MedicineInventoryForm : Form
{
    private readonly IMedicineService _medicineService;

    // Controls
    private DataGridView dgvMedicines = null!;
    private TextBox txtName = null!;
    private NumericUpDown nudPrice = null!;
    private NumericUpDown nudQuantity = null!;
    private Button btnAdd = null!;
    private Button btnDelete = null!;
    private Label lblMessage = null!;

    private List<Medicine> _medicines = new();

    public MedicineInventoryForm(IMedicineService medicineService)
    {
        _medicineService = medicineService;
        InitializeComponent();
        LoadMedicines();
    }

    private void InitializeComponent()
    {
        Text = "Medicine Inventory";
        Width = 700;
        Height = 600;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        // DataGridView
        dgvMedicines = new DataGridView
        {
            Left = 20,
            Top = 20,
            Width = 660,
            Height = 250,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        dgvMedicines.Columns.Add("Id", "ID");
        dgvMedicines.Columns.Add("Name", "Name");
        dgvMedicines.Columns.Add("Price", "Price (₪)");
        dgvMedicines.Columns.Add("Quantity", "Quantity");
        Controls.Add(dgvMedicines);

        var y = 280;

        // Add medicine section
        var lblSection = new Label { Text = "Add New Medicine:", Left = 20, Top = y, Width = 200, Font = new Font(Font, FontStyle.Bold) };
        Controls.Add(lblSection);
        y += 30;

        var lblName = new Label { Text = "Name:", Left = 20, Top = y, Width = 80 };
        txtName = new TextBox { Left = 110, Top = y, Width = 250 };
        Controls.Add(lblName);
        Controls.Add(txtName);
        y += 35;

        var lblPrice = new Label { Text = "Price (₪):", Left = 20, Top = y, Width = 80 };
        nudPrice = new NumericUpDown { Left = 110, Top = y, Width = 120, DecimalPlaces = 2, Minimum = 0.01m };
        Controls.Add(lblPrice);
        Controls.Add(nudPrice);
        y += 35;

        var lblQuantity = new Label { Text = "Quantity:", Left = 20, Top = y, Width = 80 };
        nudQuantity = new NumericUpDown { Left = 110, Top = y, Width = 120, Minimum = 0, Maximum = 10000 };
        Controls.Add(lblQuantity);
        Controls.Add(nudQuantity);
        y += 35;

        btnAdd = new Button { Text = "Add Medicine", Left = 110, Top = y, Width = 120 };
        btnAdd.Click += btnAdd_Click;
        Controls.Add(btnAdd);
        y += 35;

        btnDelete = new Button { Text = "Delete Selected", Left = 240, Top = y, Width = 120 };
        btnDelete.Click += btnDelete_Click;
        // Only enable delete if user is a veterinarian
        btnDelete.Enabled = CurrentUserSession.IsVeterinarian;
        Controls.Add(btnDelete);

        y += 35;

        lblMessage = new Label
        {
            Left = 20,
            Top = y,
            Width = 660,
            Height = 40,
            ForeColor = Color.DarkGreen,
            AutoSize = false,
            Text = string.Empty
        };
        Controls.Add(lblMessage);
    }

    private void LoadMedicines()
    {
        try
        {
            _medicines = _medicineService.GetAll().ToList();
            dgvMedicines.Rows.Clear();

            foreach (var med in _medicines)
            {
                dgvMedicines.Rows.Add(med.Id, med.Name, med.Price.ToString("F2"), med.Quantity);
            }
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = $"Error loading medicines: {ex.Message}";
        }
    }

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        lblMessage.Text = string.Empty;

        if (!MedicineValidator.ValidateName(txtName.Text, out var nameError))
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = nameError;
            return;
        }

        if (!MedicineValidator.ValidatePrice((decimal)nudPrice.Value, out var priceError))
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = priceError;
            return;
        }

        if (!MedicineValidator.ValidateQuantity((int)nudQuantity.Value, out var qtyError))
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = qtyError;
            return;
        }

        var medicine = new Medicine
        {
            Name = txtName.Text,
            Price = (decimal)nudPrice.Value,
            Quantity = (int)nudQuantity.Value
        };

        if (_medicineService.AddMedicine(medicine, out var error))
        {
            lblMessage.ForeColor = Color.DarkGreen;
            lblMessage.Text = "Medicine added successfully.";
            txtName.Clear();
            nudPrice.Value = nudPrice.Minimum;
            nudQuantity.Value = 0;
            LoadMedicines();
        }
        else
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = error;
        }
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (dgvMedicines.SelectedRows.Count == 0)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = "Please select a medicine to delete.";
            return;
        }

        var selectedRow = dgvMedicines.SelectedRows[0];
        if (int.TryParse(selectedRow.Cells[0].Value?.ToString(), out var medicineId))
        {
            if (MessageBox.Show(
                "Are you sure you want to delete this medicine?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_medicineService.DeleteMedicine(medicineId))
                {
                    lblMessage.ForeColor = Color.DarkGreen;
                    lblMessage.Text = "Medicine deleted successfully.";
                    LoadMedicines();
                }
                else
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Failed to delete medicine.";
                }
            }
        }
    }
}
