using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Session;
using ClinicVets.g1.Forms;
using ClinicVets.g2.Forms;
using ClinicVets.g3.Forms;

namespace ClinicVets.App.Forms;

/// <summary>
/// Main navigation hub shown after a successful login.
/// Buttons are grouped by role:
///   Secretary only  → Customer Management
///   All staff       → Animal Management
///   Vet only        → Visits and Medicines
/// </summary>
public class MainForm : Form
{
    private readonly IAuthService       _authService;
    private readonly ICustomerService   _customerService;
    private readonly IAnimalService     _animalService;
    private readonly IVisitService      _visitService;
    private readonly IMedicineService   _medicineService;
    private readonly ICustomerRepository _customerRepository;

    // Groups hidden/shown based on role
    private GroupBox _gbCustomers = null!;
    private GroupBox _gbVisits    = null!;

    public MainForm(
        IAuthService       authService,
        ICustomerService   customerService,
        IAnimalService     animalService,
        IVisitService      visitService,
        IMedicineService   medicineService,
        ICustomerRepository customerRepository)
    {
        _authService        = authService;
        _customerService    = customerService;
        _animalService      = animalService;
        _visitService       = visitService;
        _medicineService    = medicineService;
        _customerRepository = customerRepository;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // ── Form properties ──────────────────────────────────────────────────
        Text            = "ClinicVets – Dashboard";
        Size            = new Size(820, 600);
        MinimumSize     = new Size(820, 600);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = Color.FromArgb(245, 245, 245);
        Font            = new Font("Segoe UI", 9.5f);

        // ── Header ───────────────────────────────────────────────────────────
        var pnlHeader = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 70,
            BackColor = Color.FromArgb(21, 101, 192),
        };

        var lblAppTitle = new Label
        {
            Text      = "ClinicVets",
            Font      = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.White,
            Location  = new Point(16, 18),
            Size      = new Size(200, 36),
        };

        string welcomeText = CurrentUserSession.CurrentUser is { } u
            ? $"Welcome, {u.FullName}  |  Role: {u.Role}"
            : "Welcome";

        var lblWelcome = new Label
        {
            Text      = welcomeText,
            Font      = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(200, 230, 255),
            Location  = new Point(220, 26),
            Size      = new Size(440, 22),
        };

        var btnLogout = new Button
        {
            Text      = "Logout",
            Location  = new Point(690, 18),
            Size      = new Size(100, 34),
            BackColor = Color.FromArgb(198, 40, 40),
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += (_, _) => Close();

        pnlHeader.Controls.AddRange(new Control[] { lblAppTitle, lblWelcome, btnLogout });

        // ── Section: Customer Management (Secretary only) ─────────────────────
        _gbCustomers = new GroupBox
        {
            Text     = "Customer Management  (Secretary only)",
            Location = new Point(20, 88),
            Size     = new Size(770, 100),
            Font     = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(46, 125, 50),
        };

        var btnRegCust   = MakeNavButton("Register Customer",  Color.FromArgb(46, 125, 50),  new Point(16, 30));
        var btnSrchCust  = MakeNavButton("Search Customer",    Color.FromArgb(46, 125, 50),  new Point(186, 30));
        btnRegCust.Click  += (_, _) => OpenCustomerRegistration();
        btnSrchCust.Click += (_, _) => OpenCustomerSearch();
        _gbCustomers.Controls.AddRange(new Control[] { btnRegCust, btnSrchCust });

        // ── Section: Animal Management (All staff) ───────────────────────────
        var gbAnimals = new GroupBox
        {
            Text      = "Animal Management  (All Staff)",
            Location  = new Point(20, 202),
            Size      = new Size(770, 100),
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(21, 101, 192),
        };

        var btnAddAnimal  = MakeNavButton("Add Animal",           Color.FromArgb(21, 101, 192), new Point(16,  30));
        var btnSrchAnimal = MakeNavButton("Search Animal",        Color.FromArgb(21, 101, 192), new Point(186, 30));
        var btnCatalog    = MakeNavButton("Animal Type Catalog",  Color.FromArgb(21, 101, 192), new Point(356, 30));
        btnAddAnimal.Click    += (_, _) => OpenAddAnimal();
        btnSrchAnimal.Click   += (_, _) => OpenAnimalSearch();
        btnCatalog.Click      += (_, _) => OpenAnimalTypesCatalog();
        gbAnimals.Controls.AddRange(new Control[] { btnAddAnimal, btnSrchAnimal, btnCatalog });

        // ── Section: Visits & Medicines (Vet only) ───────────────────────────
        _gbVisits = new GroupBox
        {
            Text      = "Visits and Treatments  (Veterinarian only)",
            Location  = new Point(20, 316),
            Size      = new Size(770, 140),
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(123, 31, 162),
        };

        var btnOpenVisit    = MakeNavButton("Open Visit",          Color.FromArgb(123, 31, 162), new Point(16,  30));
        var btnMedicine     = MakeNavButton("Medicine Inventory",  Color.FromArgb(123, 31, 162), new Point(186, 30));
        var btnAnimalVisits = MakeNavButton("Animal Visits",       Color.FromArgb(123, 31, 162), new Point(16,  80));
        var btnAllVisits    = MakeNavButton("All Clinic Visits",   Color.FromArgb(123, 31, 162), new Point(186, 80));
        btnOpenVisit.Click    += (_, _) => OpenVisit();
        btnMedicine.Click     += (_, _) => OpenMedicineInventory();
        btnAnimalVisits.Click += (_, _) => OpenVisitHistory();
        btnAllVisits.Click    += (_, _) => OpenAllVisits();
        _gbVisits.Controls.AddRange(new Control[] { btnOpenVisit, btnMedicine, btnAnimalVisits, btnAllVisits });

        // ── Status bar ───────────────────────────────────────────────────────
        var pnlStatus = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = Color.FromArgb(224, 224, 224) };
        var lblStatus = new Label
        {
            Text      = "  ClinicVets v1.0  |  Database: SQLite  |  Ready",
            Font      = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(66, 66, 66),
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        pnlStatus.Controls.Add(lblStatus);

        // ── Build form ───────────────────────────────────────────────────────
        Controls.Add(pnlHeader);
        Controls.Add(_gbCustomers);
        Controls.Add(gbAnimals);
        Controls.Add(_gbVisits);
        Controls.Add(pnlStatus);

        Load += (_, _) => ApplyRoleVisibility();
    }

    private static Button MakeNavButton(string text, Color color, Point location) => new()
    {
        Text      = text,
        Location  = location,
        Size      = new Size(160, 44),
        BackColor = color,
        ForeColor = Color.White,
        Font      = new Font("Segoe UI", 10, FontStyle.Bold),
        FlatStyle = FlatStyle.Flat,
        Cursor    = Cursors.Hand,
    };

    private void ApplyRoleVisibility()
    {
        // Customer management: Secretary only
        _gbCustomers.Visible = CurrentUserSession.IsSecretary;

        // Visits and treatments: Veterinarian only
        _gbVisits.Visible = CurrentUserSession.IsVeterinarian;
    }

    // ── Navigation helpers ───────────────────────────────────────────────────

    private void OpenCustomerRegistration()
    {
        if (!CurrentUserSession.IsSecretary)
        {
            MessageBox.Show("Access denied. Secretary role required.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        new CustomerRegistrationForm(_customerService).ShowDialog(this);
    }

    private void OpenCustomerSearch()
    {
        if (!CurrentUserSession.IsSecretary)
        {
            MessageBox.Show("Access denied. Secretary role required.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        new CustomerSearchForm(_customerService).ShowDialog(this);
    }

    private void OpenAddAnimal()
        => new AddAnimalForm(_animalService, _customerRepository).ShowDialog(this);

    private void OpenAnimalSearch()
        => new AnimalSearchForm(_animalService).ShowDialog(this);

    private void OpenAnimalTypesCatalog()
        => new AnimalTypesCatalogForm().ShowDialog(this);

private void OpenVisit()
    {
        if (!CurrentUserSession.IsVeterinarian)
        {
            MessageBox.Show("Access denied. Veterinarian role required.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        new OpenVisitForm(_visitService, _animalService, _medicineService).ShowDialog(this);
    }

    private void OpenMedicineInventory()
        => new MedicineInventoryForm(_medicineService).ShowDialog(this);

    private void OpenVisitHistory()
        => new VisitHistoryForm(_visitService, _animalService).ShowDialog(this);

    private void OpenAllVisits()
        => new AllVisitsForm(_visitService, _animalService).ShowDialog(this);
}
