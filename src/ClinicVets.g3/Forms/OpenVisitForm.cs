using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g3.Forms;

/// <summary>
/// Screen 8 — Open a new visit for an existing animal.
/// Accessible only when CurrentUserSession.Role == Veterinarian.
///
/// Fields: Animal selector (ComboBox or search), Reason, Date (default=today),
///         Time (default=now), Diagnosis, TreatingVet (auto-filled from session),
///         Medicine list (CheckedListBox from inventory), Cost summary (read-only label).
///
/// TODO (g3 – Yousef Naameh):
///   - Guard the form: redirect non-vets to an access-denied message.
///   - Populate the animal ComboBox from IAnimalRepository.GetAll().
///   - On Save: collect selected medicines, call _visitService.OpenVisit(),
///     display the calculated cost and any vaccination alert.
/// </summary>
public class OpenVisitForm : Form
{
    private readonly IVisitService _visitService;
    private readonly IAnimalService _animalService;
    private readonly IMedicineService _medicineService;

    public OpenVisitForm(
        IVisitService visitService,
        IAnimalService animalService,
        IMedicineService medicineService)
    {
        _visitService = visitService;
        _animalService = animalService;
        _medicineService = medicineService;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnSave_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();

    /// <summary>
    /// Checks NeedsVaccination on the selected animal and shows a MessageBox warning
    /// if annual vaccination is overdue.
    /// </summary>
    private void CheckVaccinationAlert(Animal animal)
        => throw new NotImplementedException();
}
