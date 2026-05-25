using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.Core.Session;

namespace ClinicVets.g3.Forms;

public class OpenVisitForm : Form
{
    private readonly IVisitService _visitService;
    private readonly IAnimalService _animalService;
    private readonly IMedicineService _medicineService;

    // Controls
    private ComboBox cmbAnimal = null!;
    private TextBox txtReason = null!;
    private DateTimePicker dtVisitDate = null!;
    private DateTimePicker dtVisitTime = null!;
    private TextBox txtDiagnosis = null!;
    private TextBox txtVetName = null!;
    private CheckedListBox lstMedicines = null!;
    private Label lblTotalCost = null!;
    private Label lblErrorMessage = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;

    private List<Animal> _allAnimals = new();
    private List<Medicine> _availableMedicines = new();

    public OpenVisitForm(
        IVisitService visitService,
        IAnimalService animalService,
        IMedicineService medicineService)
    {
        _visitService = visitService;
        _animalService = animalService;
        _medicineService = medicineService;

        // Guard: only veterinarians can open visits
        if (!CurrentUserSession.IsVeterinarian)
        {
            throw new UnauthorizedAccessException("Only veterinarians can open visits.");
        }

        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Text = "Open New Visit";
        Width = 500;
        Height = 650;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var y = 20;

        // Animal selection
        var lblAnimal = new Label { Text = "Animal:", Left = 20, Top = y, Width = 100 };
        cmbAnimal = new ComboBox
        {
            Left = 150,
            Top = y,
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(lblAnimal);
        Controls.Add(cmbAnimal);
        y += 40;

        // Reason
        var lblReason = new Label { Text = "Reason:", Left = 20, Top = y, Width = 100 };
        txtReason = new TextBox { Left = 150, Top = y, Width = 300, Height = 60, Multiline = true };
        Controls.Add(lblReason);
        Controls.Add(txtReason);
        y += 80;

        // Visit date (default today)
        var lblDate = new Label { Text = "Visit Date:", Left = 20, Top = y, Width = 100 };
        dtVisitDate = new DateTimePicker { Left = 150, Top = y, Width = 150, Value = DateTime.Today };
        Controls.Add(lblDate);
        Controls.Add(dtVisitDate);
        y += 40;

        // Visit time (default now)
        var lblTime = new Label { Text = "Visit Time:", Left = 20, Top = y, Width = 100 };
        dtVisitTime = new DateTimePicker
        {
            Left = 150,
            Top = y,
            Width = 150,
            Value = DateTime.Now,
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true
        };
        Controls.Add(lblTime);
        Controls.Add(dtVisitTime);
        y += 40;

        // Diagnosis
        var lblDiagnosis = new Label { Text = "Diagnosis:", Left = 20, Top = y, Width = 100 };
        txtDiagnosis = new TextBox { Left = 150, Top = y, Width = 300, Height = 60, Multiline = true };
        Controls.Add(lblDiagnosis);
        Controls.Add(txtDiagnosis);
        y += 80;

        // Vet name (auto-filled, read-only)
        var lblVetName = new Label { Text = "Treating Vet:", Left = 20, Top = y, Width = 100 };
        txtVetName = new TextBox { Left = 150, Top = y, Width = 300, ReadOnly = true };
        if (CurrentUserSession.CurrentUser != null)
        {
            txtVetName.Text = CurrentUserSession.CurrentUser.FullName;
        }
        Controls.Add(lblVetName);
        Controls.Add(txtVetName);
        y += 40;

        // Medicines list
        var lblMedicines = new Label { Text = "Medicines:", Left = 20, Top = y, Width = 100 };
        lstMedicines = new CheckedListBox { Left = 150, Top = y, Width = 300, Height = 80 };
        Controls.Add(lblMedicines);
        Controls.Add(lstMedicines);
        y += 100;

        // Total cost (read-only)
        var lblCostLabel = new Label { Text = "Total Cost:", Left = 20, Top = y, Width = 100 };
        lblTotalCost = new Label { Left = 150, Top = y, Width = 300, Text = "₪ 100.00" };
        Controls.Add(lblCostLabel);
        Controls.Add(lblTotalCost);
        y += 40;

        // Error message label
        lblErrorMessage = new Label
        {
            Left = 20,
            Top = y,
            Width = 450,
            Height = 50,
            ForeColor = Color.Red,
            AutoSize = false,
            Text = string.Empty
        };
        Controls.Add(lblErrorMessage);
        y += 60;

        // Buttons
        btnSave = new Button { Text = "Save Visit", Left = 150, Top = y, Width = 100 };
        btnCancel = new Button { Text = "Cancel", Left = 270, Top = y, Width = 100 };
        btnSave.Click += btnSave_Click;
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        Controls.Add(btnSave);
        Controls.Add(btnCancel);

        // When medicine selection changes, recalculate cost
        lstMedicines.ItemCheck += (_, _) => RecalculateCost();
    }

    private void LoadData()
    {
        try
        {
            // Load all animals via SearchByName("") to get all; adjust if g2 implements GetAll
            _allAnimals = _animalService.SearchByName("").ToList();
            if (_allAnimals.Count == 0)
            {
                lblErrorMessage.Text = "No animals found. Please register animals first.";
            }

            foreach (var animal in _allAnimals)
            {
                cmbAnimal.Items.Add($"{animal.Name} (Chip: {animal.ChipNumber})");
            }

            // Load all medicines
            _availableMedicines = _medicineService.GetAll().ToList();
            foreach (var med in _availableMedicines)
            {
                lstMedicines.Items.Add($"{med.Name} (₪{med.Price})", false);
            }

            RecalculateCost();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = $"Error loading data: {ex.Message}";
        }
    }

    private void RecalculateCost()
    {
        var selectedMedicines = lstMedicines.CheckedIndices
            .Cast<int>()
            .Select(i => _availableMedicines[i])
            .ToList();

        var total = _visitService.CalculateTotalCost(100m, selectedMedicines);
        lblTotalCost.Text = $"₪ {total:F2}";
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        lblErrorMessage.Text = string.Empty;

        if (cmbAnimal.SelectedIndex < 0)
        {
            lblErrorMessage.Text = "Please select an animal.";
            return;
        }

        var selectedAnimal = _allAnimals[cmbAnimal.SelectedIndex];
        var visitDateTime = dtVisitDate.Value.Date + dtVisitTime.Value.TimeOfDay;

        var selectedMedicines = lstMedicines.CheckedIndices
            .Cast<int>()
            .Select(i => _availableMedicines[i])
            .ToList();

        var visit = new Visit
        {
            AnimalId = selectedAnimal.Id,
            Reason = txtReason.Text,
            VisitDateTime = visitDateTime,
            Diagnosis = txtDiagnosis.Text,
            VetEmployeeId = CurrentUserSession.CurrentUser?.Id ?? 0,
            Medicines = selectedMedicines
        };

        if (_visitService.OpenVisit(visit, out var error))
        {
            MessageBox.Show("Visit saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
        else
        {
            lblErrorMessage.Text = error;
        }
    }
}
