using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g2.Forms;

/// <summary>
/// Screen 6 - Search for an animal by name or chip number.
/// Accessible to all staff roles.
/// </summary>
public class AnimalSearchForm : Form
{
    private readonly IAnimalService _animalService;

    private TextBox txtName = null!;
    private TextBox txtChip = null!;
    private Button btnSearch = null!;
    private DataGridView gridResults = null!;
    private TextBox txtDetails = null!;
    private List<Animal> currentResults = [];

    public AnimalSearchForm(IAnimalService animalService)
    {
        _animalService = animalService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Animal Search";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(820, 560);

        var title = new Label
        {
            Text = "Search Animals",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 14F, FontStyle.Bold),
            Location = new Point(24, 20)
        };

        var searchPanel = new TableLayoutPanel
        {
            ColumnCount = 5,
            RowCount = 1,
            Location = new Point(24, 62),
            Size = new Size(756, 42),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        txtName = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, MaxLength = 40 };
        txtChip = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, MaxLength = 40 };
        txtName.KeyDown += SearchOnEnter;
        txtChip.KeyDown += SearchOnEnter;

        btnSearch = new Button
        {
            Text = "Search",
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Height = 30
        };
        btnSearch.Click += btnSearch_Click;

        searchPanel.Controls.Add(new Label { Text = "Name", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        searchPanel.Controls.Add(txtName, 1, 0);
        searchPanel.Controls.Add(new Label { Text = "Chip number", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
        searchPanel.Controls.Add(txtChip, 3, 0);
        searchPanel.Controls.Add(btnSearch, 4, 0);

        gridResults = new DataGridView
        {
            Location = new Point(24, 124),
            Size = new Size(756, 245),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false
        };
        gridResults.CellDoubleClick += gridResults_CellDoubleClick;
        gridResults.SelectionChanged += gridResults_SelectionChanged;

        txtDetails = new TextBox
        {
            Location = new Point(24, 388),
            Size = new Size(756, 94),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical
        };

        Controls.Add(title);
        Controls.Add(searchPanel);
        Controls.Add(gridResults);
        Controls.Add(txtDetails);
    }

    private void btnSearch_Click(object? sender, EventArgs e)
    {
        var name = txtName.Text.Trim();
        var chip = txtChip.Text.Trim();

        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(chip))
        {
            MessageBox.Show(
                this,
                "Enter a name or chip number to search.",
                "Search",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        currentResults = [];

        if (!string.IsNullOrWhiteSpace(chip))
        {
            var animal = _animalService.SearchByChipNumber(chip);
            if (animal is not null)
            {
                currentResults.Add(animal);
            }
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var byName = _animalService.SearchByName(name);
            currentResults.AddRange(byName.Where(animal => currentResults.All(existing => existing.Id != animal.Id)));
        }

        BindResults();
    }

    private void BindResults()
    {
        gridResults.DataSource = currentResults
            .Select(animal => new AnimalSearchRow(
                animal.Id,
                animal.ChipNumber,
                animal.Name,
                animal.Type.ToString(),
                animal.Weight,
                animal.BirthDate.ToShortDateString(),
                animal.OwnerId,
                animal.LastVaccinationDate?.ToShortDateString() ?? "Not recorded",
                _animalService.NeedsVaccination(animal) ? "Yes" : "No"))
            .ToList();

        txtDetails.Clear();

        if (currentResults.Count == 0)
        {
            MessageBox.Show(this, "No animals matched the search.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void gridResults_SelectionChanged(object? sender, EventArgs e)
    {
        var animal = GetSelectedAnimal();
        txtDetails.Text = animal is null ? string.Empty : FormatDetails(animal);
    }

    private void gridResults_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        var animal = GetSelectedAnimal();
        if (animal is null)
        {
            return;
        }

        MessageBox.Show(this, FormatDetails(animal), "Animal Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private Animal? GetSelectedAnimal()
    {
        if (gridResults.CurrentRow?.Index is not int index || index < 0 || index >= currentResults.Count)
        {
            return null;
        }

        return currentResults[index];
    }

    private string FormatDetails(Animal animal)
        => $"Chip: {animal.ChipNumber}{Environment.NewLine}"
            + $"Name: {animal.Name}{Environment.NewLine}"
            + $"Type: {animal.Type}{Environment.NewLine}"
            + $"Weight: {animal.Weight:0.##} kg{Environment.NewLine}"
            + $"Birth date: {animal.BirthDate:d}{Environment.NewLine}"
            + $"Owner ID: {animal.OwnerId}{Environment.NewLine}"
            + $"Last vaccination: {(animal.LastVaccinationDate.HasValue ? animal.LastVaccinationDate.Value.ToShortDateString() : "Not recorded")}{Environment.NewLine}"
            + $"Needs vaccination: {(_animalService.NeedsVaccination(animal) ? "Yes" : "No")}";

    private void SearchOnEnter(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        e.SuppressKeyPress = true;
        btnSearch.PerformClick();
    }

    private sealed record AnimalSearchRow(
        int Id,
        string ChipNumber,
        string Name,
        string Type,
        decimal Weight,
        string BirthDate,
        int OwnerId,
        string LastVaccinationDate,
        string NeedsVaccination);
}
