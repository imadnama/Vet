using System.ComponentModel;
using ClinicVets.Core.Enums;

namespace ClinicVets.g2.Forms;

/// <summary>
/// Screen 7 - Animal type catalog view.
/// Accessible to all staff roles.
/// </summary>
public class AnimalTypesCatalogForm : Form
{
    private readonly Action<AnimalType>? openAddAnimal;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AnimalType? SelectedAnimalType { get; private set; }

    public AnimalTypesCatalogForm()
        : this(null)
    {
    }

    public AnimalTypesCatalogForm(Action<AnimalType>? openAddAnimal)
    {
        this.openAddAnimal = openAddAnimal;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Animal Types";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(700, 470);

        var title = new Label
        {
            Text = "Animal Type Catalog",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 14F, FontStyle.Bold),
            Location = new Point(24, 20)
        };

        var tabs = new TabControl
        {
            Location = new Point(24, 62),
            Size = new Size(636, 330),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        foreach (var type in Enum.GetValues<AnimalType>())
        {
            tabs.TabPages.Add(CreateTypePage(type));
        }

        Controls.Add(title);
        Controls.Add(tabs);
    }

    private TabPage CreateTypePage(AnimalType type)
    {
        var page = new TabPage(type.ToString());
        var profile = GetProfile(type);

        var grid = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 5,
            Dock = DockStyle.Top,
            Height = 220,
            Padding = new Padding(18),
            AutoSize = false
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddProfileRow(grid, 0, "Category", type.ToString());
        AddProfileRow(grid, 1, "Common patients", profile.CommonPatients);
        AddProfileRow(grid, 2, "Typical weight", profile.TypicalWeight);
        AddProfileRow(grid, 3, "Vaccination note", profile.VaccinationNote);
        AddProfileRow(grid, 4, "Handling note", profile.HandlingNote);

        var addButton = new Button
        {
            Text = $"Add {type}",
            Width = 120,
            Height = 34,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        addButton.Click += (_, _) => OpenAddForType(type);

        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            Padding = new Padding(18, 8, 18, 12),
            FlowDirection = FlowDirection.RightToLeft
        };
        footer.Controls.Add(addButton);

        page.Controls.Add(grid);
        page.Controls.Add(footer);
        return page;
    }

    private static void AddProfileRow(TableLayoutPanel grid, int rowIndex, string label, string value)
    {
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        grid.Controls.Add(
            new Label
            {
                Text = label,
                AutoSize = true,
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
                Anchor = AnchorStyles.Left
            },
            0,
            rowIndex);
        grid.Controls.Add(
            new Label
            {
                Text = value,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            },
            1,
            rowIndex);
    }

    private void OpenAddForType(AnimalType type)
    {
        SelectedAnimalType = type;

        if (openAddAnimal is not null)
        {
            openAddAnimal(type);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private static AnimalTypeProfile GetProfile(AnimalType type)
        => type switch
        {
            AnimalType.Dog => new AnimalTypeProfile(
                "Dogs and puppies",
                "1-80 kg",
                "Annual vaccination tracking is recommended.",
                "Use calm handling and confirm leash control."),
            AnimalType.Cat => new AnimalTypeProfile(
                "Cats and kittens",
                "0.5-12 kg",
                "Annual vaccination tracking is recommended.",
                "Use a quiet room and secure carrier transfer."),
            AnimalType.Reptile => new AnimalTypeProfile(
                "Lizards, snakes and turtles",
                "0.1-30 kg",
                "Vaccination is uncommon; record owner-provided history.",
                "Confirm temperature and enclosure needs with the owner."),
            AnimalType.Bird => new AnimalTypeProfile(
                "Parrots, canaries and other birds",
                "0.1-5 kg",
                "Record prevention history when supplied by the owner.",
                "Minimize handling time and avoid drafts."),
            _ => new AnimalTypeProfile("Unknown", "Unknown", "Unknown", "Unknown")
        };

    private sealed record AnimalTypeProfile(
        string CommonPatients,
        string TypicalWeight,
        string VaccinationNote,
        string HandlingNote);
}
