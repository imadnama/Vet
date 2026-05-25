using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g2.Forms;

/// <summary>
/// Screen 5 - Add a new animal patient.
/// Accessible to all staff roles.
/// </summary>
public class AddAnimalForm : Form
{
    private readonly IAnimalService _animalService;
    private readonly ICustomerRepository _customers;

    private TextBox txtName = null!;
    private ComboBox cmbType = null!;
    private NumericUpDown numWeight = null!;
    private DateTimePicker dtpBirthDate = null!;
    private ComboBox cmbOwner = null!;
    private DateTimePicker dtpLastVaccination = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;

    public AddAnimalForm(IAnimalService animalService, ICustomerRepository customers)
        : this(animalService, customers, null)
    {
    }

    public AddAnimalForm(IAnimalService animalService, ICustomerRepository customers, AnimalType? preselectedType)
    {
        _animalService = animalService;
        _customers = customers;
        InitializeComponent();

        if (preselectedType.HasValue)
        {
            cmbType.SelectedItem = preselectedType.Value;
        }

        LoadOwners();
    }

    private void InitializeComponent()
    {
        Text = "Add Animal";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(520, 460);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var title = new Label
        {
            Text = "New Animal Patient",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 14F, FontStyle.Bold),
            Location = new Point(24, 20)
        };

        var layout = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 6,
            Location = new Point(24, 62),
            Size = new Size(456, 260),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            AutoSize = false
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        txtName = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, MaxLength = 40 };
        cmbType = new ComboBox
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList,
            DataSource = Enum.GetValues<AnimalType>()
        };
        numWeight = new NumericUpDown
        {
            Anchor = AnchorStyles.Left,
            DecimalPlaces = 2,
            Minimum = 0.1m,
            Maximum = 100m,
            Increment = 0.1m,
            Width = 120
        };
        dtpBirthDate = new DateTimePicker
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Format = DateTimePickerFormat.Short,
            MinDate = new DateTime(2000, 1, 1),
            MaxDate = DateTime.Today,
            Value = DateTime.Today
        };
        cmbOwner = new ComboBox
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        dtpLastVaccination = new DateTimePicker
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Format = DateTimePickerFormat.Short,
            MaxDate = DateTime.Today,
            ShowCheckBox = true,
            Checked = false
        };

        AddRow(layout, 0, "Name", txtName);
        AddRow(layout, 1, "Type", cmbType);
        AddRow(layout, 2, "Weight (kg)", numWeight);
        AddRow(layout, 3, "Birth date", dtpBirthDate);
        AddRow(layout, 4, "Owner", cmbOwner);
        AddRow(layout, 5, "Last vaccination", dtpLastVaccination);

        btnSave = new Button
        {
            Text = "Save",
            Size = new Size(100, 34),
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom
        };
        btnSave.Click += btnSave_Click;

        btnCancel = new Button
        {
            Text = "Cancel",
            Size = new Size(100, 34),
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
            DialogResult = DialogResult.Cancel
        };

        var buttons = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Location = new Point(24, 344),
            Size = new Size(456, 44),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
        };
        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);

        AcceptButton = btnSave;
        CancelButton = btnCancel;
        Controls.Add(title);
        Controls.Add(layout);
        Controls.Add(buttons);
    }

    private static void AddRow(TableLayoutPanel layout, int rowIndex, string labelText, Control editor)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        var label = new Label
        {
            Text = labelText,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 8, 12, 0)
        };

        editor.Margin = new Padding(0, 4, 0, 4);
        layout.Controls.Add(label, 0, rowIndex);
        layout.Controls.Add(editor, 1, rowIndex);
    }

    private void LoadOwners()
    {
        var owners = _customers
            .GetAll()
            .OrderBy(customer => customer.FullName)
            .Select(customer => new OwnerListItem(customer.Id, customer.FullName, customer.NationalId))
            .ToList();

        cmbOwner.DataSource = owners;
        cmbOwner.DisplayMember = nameof(OwnerListItem.DisplayText);
        cmbOwner.ValueMember = nameof(OwnerListItem.Id);
        btnSave.Enabled = owners.Count > 0;

        if (owners.Count == 0)
        {
            MessageBox.Show(
                this,
                "Add a customer before registering an animal.",
                "No customers found",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (cmbOwner.SelectedValue is not int ownerId)
        {
            MessageBox.Show(this, "Please select an owner.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var animal = new Animal
        {
            Name = txtName.Text,
            Type = (AnimalType)cmbType.SelectedItem!,
            Weight = numWeight.Value,
            BirthDate = dtpBirthDate.Value.Date,
            OwnerId = ownerId,
            LastVaccinationDate = dtpLastVaccination.Checked ? dtpLastVaccination.Value.Date : null
        };

        if (!_animalService.AddAnimal(animal, out var error))
        {
            MessageBox.Show(this, error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(
            this,
            $"Animal saved successfully.{Environment.NewLine}Chip number: {animal.ChipNumber}",
            "Saved",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record OwnerListItem(int Id, string FullName, string NationalId)
    {
        public string DisplayText => $"{FullName} ({NationalId})";
    }
}
