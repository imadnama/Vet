using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g2.Forms;

/// <summary>
/// Screen 5 — Add a new animal patient.
/// Accessible to all staff roles.
///
/// Fields: Name (letters), Type (ComboBox: Dog/Cat/Reptile/Bird),
///         Weight (decimal), BirthDate (DateTimePicker),
///         Owner (ComboBox populated from existing customers),
///         LastVaccinationDate (DateTimePicker, optional).
///
/// TODO (g2 – Ahmad Abu Zaid):
///   - Build the layout in InitializeComponent().
///   - Populate the owner ComboBox from ICustomerRepository.GetAll().
///   - On Save: validate via AnimalValidator, then call _animalService.AddAnimal().
/// </summary>
public class AddAnimalForm : Form
{
    private readonly IAnimalService _animalService;
    private readonly ICustomerRepository _customers;

    public AddAnimalForm(IAnimalService animalService, ICustomerRepository customers)
    {
        _animalService = animalService;
        _customers = customers;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnSave_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();
}
