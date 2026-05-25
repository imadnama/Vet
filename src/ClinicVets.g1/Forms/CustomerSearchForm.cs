using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Screen 4 — Search customers and view their linked animals.
/// Accessible only when the current user is a Secretary.
/// Role enforcement is applied in MainForm before this form is opened.
/// </summary>
public class CustomerSearchForm : Form
{
    private readonly ICustomerService _customerService;

    private TextBox         _txtSearchId    = null!;
    private TextBox         _txtSearchPhone = null!;
    private Label           _lblName        = null!;
    private Label           _lblId          = null!;
    private Label           _lblPhone       = null!;
    private Label           _lblEmail       = null!;
    private DataGridView    _dgvAnimals     = null!;
    private Label           _lblStatus      = null!;

    public CustomerSearchForm(ICustomerService customerService)
    {
        _customerService = customerService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // ── Form properties ──────────────────────────────────────────────────
        Text            = "ClinicVets – Customer Search";
        Size            = new Size(760, 620);
        MinimumSize     = new Size(760, 620);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = Color.White;
        Font            = new Font("Segoe UI", 9.5f);

        // ── Header ───────────────────────────────────────────────────────────
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(46, 125, 50) };
        var lblTitle  = new Label { Text = "Customer Search", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
        pnlHeader.Controls.Add(lblTitle);

        // ── Search section ───────────────────────────────────────────────────
        int x = 20;

        var lblSrchId = new Label { Text = "Search by National ID:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(x, 72), Size = new Size(170, 20) };
        _txtSearchId  = new TextBox { Location = new Point(x, 94), Size = new Size(185, 30), Font = new Font("Segoe UI", 10), MaxLength = 9 };

        var btnById   = new Button
        {
            Text      = "Search by ID",
            Location  = new Point(x + 190, 93),
            Size      = new Size(115, 32),
            BackColor = Color.FromArgb(46, 125, 50),
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
        };
        btnById.FlatAppearance.BorderSize = 0;
        btnById.Click += (_, _) => SearchAndDisplay(byId: true);

        var lblSrchPh = new Label { Text = "Search by Phone:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(330, 72), Size = new Size(140, 20) };
        _txtSearchPhone = new TextBox { Location = new Point(330, 94), Size = new Size(185, 30), Font = new Font("Segoe UI", 10), MaxLength = 14 };

        var btnByPh = new Button
        {
            Text      = "Search by Phone",
            Location  = new Point(520, 93),
            Size      = new Size(120, 32),
            BackColor = Color.FromArgb(46, 125, 50),
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
        };
        btnByPh.FlatAppearance.BorderSize = 0;
        btnByPh.Click += (_, _) => SearchAndDisplay(byId: false);

        // ── Customer info panel (GroupBox) ───────────────────────────────────
        var gbCustomer = new GroupBox
        {
            Text     = "Customer Details",
            Location = new Point(x, 138),
            Size     = new Size(715, 100),
            Font     = new Font("Segoe UI", 9, FontStyle.Bold),
        };

        _lblName  = MakeInfoLabel("Name: —",   new Point(10, 22));
        _lblId    = MakeInfoLabel("ID: —",      new Point(10, 46));
        _lblPhone = MakeInfoLabel("Phone: —",   new Point(300, 22));
        _lblEmail = MakeInfoLabel("Email: —",   new Point(300, 46));
        gbCustomer.Controls.AddRange(new Control[] { _lblName, _lblId, _lblPhone, _lblEmail });

        // ── Status label ─────────────────────────────────────────────────────
        _lblStatus = new Label
        {
            Location  = new Point(x, 245),
            Size      = new Size(715, 24),
            ForeColor = Color.FromArgb(198, 40, 40),
            Font      = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleLeft,
        };

        // ── Linked animals DataGridView ───────────────────────────────────────
        var lblAnimals = new Label
        {
            Text     = "Linked Animals",
            Location = new Point(x, 270),
            Size     = new Size(200, 20),
            Font     = new Font("Segoe UI", 9, FontStyle.Bold),
        };

        _dgvAnimals = new DataGridView
        {
            Location          = new Point(x, 293),
            Size              = new Size(715, 280),
            ReadOnly          = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor   = Color.White,
            BorderStyle       = BorderStyle.Fixed3D,
            Font              = new Font("Segoe UI", 9),
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            },
            EnableHeadersVisualStyles = false,
        };
        BuildGridColumns();

        // ── Wire up ──────────────────────────────────────────────────────────
        Controls.Add(pnlHeader);
        foreach (var ctrl in new Control[]
        {
            lblSrchId, _txtSearchId, btnById,
            lblSrchPh, _txtSearchPhone, btnByPh,
            gbCustomer, _lblStatus,
            lblAnimals, _dgvAnimals,
        })
        {
            Controls.Add(ctrl);
        }

        // Enter key on ID box triggers search
        _txtSearchId.KeyDown    += (_, e) => { if (e.KeyCode == Keys.Return) SearchAndDisplay(byId: true); };
        _txtSearchPhone.KeyDown += (_, e) => { if (e.KeyCode == Keys.Return) SearchAndDisplay(byId: false); };
    }

    private static Label MakeInfoLabel(string text, Point location) => new()
    {
        Text     = text,
        Location = location,
        Size     = new Size(280, 20),
        Font     = new Font("Segoe UI", 9.5f),
    };

    private void BuildGridColumns()
    {
        _dgvAnimals.Columns.Clear();
        _dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name",        HeaderText = "Name",            DataPropertyName = "Name" });
        _dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type",        HeaderText = "Type",            DataPropertyName = "Type" });
        _dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChipNumber",  HeaderText = "Chip #",          DataPropertyName = "ChipNumber" });
        _dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { Name = "Weight",      HeaderText = "Weight (kg)",     DataPropertyName = "Weight" });
        _dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { Name = "BirthDate",   HeaderText = "Birth Date",      DataPropertyName = "BirthDate" });
        _dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastVaccine", HeaderText = "Last Vaccination",DataPropertyName = "LastVaccinationDate" });
    }

    private void SearchAndDisplay(bool byId)
    {
        _lblStatus.Text = string.Empty;
        ResetCustomerInfo();
        _dgvAnimals.DataSource = null;

        string term = byId
            ? _txtSearchId.Text.Trim()
            : _txtSearchPhone.Text.Trim();

        if (string.IsNullOrWhiteSpace(term))
        {
            _lblStatus.Text = "Please enter a search term.";
            return;
        }

        Customer? customer = byId
            ? _customerService.SearchByNationalId(term)
            : _customerService.SearchByPhone(term);

        if (customer == null)
        {
            _lblStatus.Text = "No customer found with the given " + (byId ? "ID." : "phone number.");
            return;
        }

        // Display customer details
        _lblName.Text  = $"Name:   {customer.FullName}";
        _lblId.Text    = $"ID:       {customer.NationalId}";
        _lblPhone.Text = $"Phone: {customer.Phone}";
        _lblEmail.Text = $"Email:   {customer.Email}";

        // Load linked animals
        var animals = _customerService.GetCustomerAnimals(customer.Id).ToList();

        var rows = animals.Select(a => new
        {
            a.Name,
            Type              = a.Type.ToString(),
            a.ChipNumber,
            Weight            = $"{a.Weight:F2}",
            BirthDate         = a.BirthDate.ToString("dd/MM/yyyy"),
            LastVaccinationDate = a.LastVaccinationDate.HasValue
                ? a.LastVaccinationDate.Value.ToString("dd/MM/yyyy")
                : "—",
        }).ToList();

        _dgvAnimals.DataSource = rows;

        _lblStatus.ForeColor = Color.FromArgb(46, 125, 50);
        _lblStatus.Text = animals.Count == 0
            ? "Customer found. No linked animals."
            : $"Customer found. {animals.Count} animal(s) linked.";
    }

    private void ResetCustomerInfo()
    {
        _lblName.Text  = "Name: —";
        _lblId.Text    = "ID: —";
        _lblPhone.Text = "Phone: —";
        _lblEmail.Text = "Email: —";
    }
}
