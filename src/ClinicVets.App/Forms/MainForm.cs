using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Session;
using ClinicVets.g1.Forms;
using ClinicVets.g2.Forms;
using ClinicVets.g3.Forms;

namespace ClinicVets.App.Forms;

/// <summary>
/// Main navigation hub shown after a successful login.
/// Displays buttons based on the current user's role:
///
///   All roles      → Animal cards (AddAnimalForm, AnimalSearchForm, AnimalTypesCatalogForm)
///   Secretary only → Customer management (CustomerRegistrationForm, CustomerSearchForm)
///   Vet only       → Visits & medicines (OpenVisitForm, MedicineInventoryForm)
///
/// TODO (shared — any pair can implement this):
///   - Build the layout: MenuStrip or a panel of buttons grouped by role.
///   - On load, hide buttons the current user is not allowed to see.
///   - Each button opens the corresponding Form using the service instances
///     supplied through the constructor.
/// </summary>
public class MainForm : Form
{
    private readonly IAuthService _authService;
    private readonly ICustomerService _customerService;
    private readonly IAnimalService _animalService;
    private readonly IVisitService _visitService;
    private readonly IMedicineService _medicineService;
    private readonly ICustomerRepository _customerRepository;

    public MainForm(
        IAuthService authService,
        ICustomerService customerService,
        IAnimalService animalService,
        IVisitService visitService,
        IMedicineService medicineService,
        ICustomerRepository customerRepository)
    {
        _authService = authService;
        _customerService = customerService;
        _animalService = animalService;
        _visitService = visitService;
        _medicineService = medicineService;
        _customerRepository = customerRepository;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void ApplyRoleVisibility()
        => throw new NotImplementedException();

    // ── Navigation helpers ──────────────────────────────────────────────────

    private void OpenCustomerRegistration()
        => new CustomerRegistrationForm(_customerService).ShowDialog(this);

    private void OpenCustomerSearch()
        => new CustomerSearchForm(_customerService).ShowDialog(this);

    private void OpenAddAnimal()
        => new AddAnimalForm(_animalService, _customerRepository).ShowDialog(this);

    private void OpenAnimalSearch()
        => new AnimalSearchForm(_animalService).ShowDialog(this);

    private void OpenAnimalCatalog()
        => new AnimalTypesCatalogForm().ShowDialog(this);

    private void OpenVisit()
        => new OpenVisitForm(_visitService, _animalService, _medicineService).ShowDialog(this);

    private void OpenMedicineInventory()
        => new MedicineInventoryForm(_medicineService).ShowDialog(this);
}
