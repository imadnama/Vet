using ClinicVets.Core.Enums;

namespace ClinicVets.g2.Forms;

/// <summary>
/// Screen 7 — Animal type catalog view.
/// Shows all animals grouped by AnimalType (Dog, Cat, Reptile, Bird).
/// Accessible to all staff roles.
///
/// TODO (g2 – Abd Alaziz Al-Baal):
///   - Display a read-only catalog (e.g. TabControl with one tab per type, or a
///     filtered DataGridView with a type ComboBox filter).
///   - Provide a shortcut button to open AddAnimalForm for a pre-selected type.
/// </summary>
public class AnimalTypesCatalogForm : Form
{
    public AnimalTypesCatalogForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
        => throw new NotImplementedException();
}
