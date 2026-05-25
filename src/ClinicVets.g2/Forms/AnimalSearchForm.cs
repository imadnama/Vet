using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g2.Forms;

/// <summary>
/// Screen 6 — Search for an animal by name or chip number.
/// Accessible to all staff roles.
///
/// TODO (g2 – Ahmad Abu Zaid):
///   - Two search inputs: txtName and txtChip, plus a Search button.
///   - Results shown in a DataGridView.
///   - Double-click on a row opens a read-only detail panel.
/// </summary>
public class AnimalSearchForm : Form
{
    private readonly IAnimalService _animalService;

    public AnimalSearchForm(IAnimalService animalService)
    {
        _animalService = animalService;
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();

    private void btnSearch_Click(object? sender, EventArgs e)
        => throw new NotImplementedException();
}
