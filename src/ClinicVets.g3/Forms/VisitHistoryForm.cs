using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g3.Forms;

/// <summary>
/// Shows visits for a specific animal, searched by name or chip number.
/// </summary>
public class VisitHistoryForm : Form
{
    private readonly IVisitService  _visitService;
    private readonly IAnimalService _animalService;

    private TextBox      txtSearch   = null!;
    private TextBox      txtChip     = null!;
    private Button       btnSearch   = null!;
    private Label        lblFound    = null!;
    private DataGridView dgvVisits   = null!;
    private TextBox      txtDetails  = null!;

    private List<Visit> _visits = new();

    public VisitHistoryForm(IVisitService visitService, IAnimalService animalService)
    {
        _visitService  = visitService;
        _animalService = animalService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "Animal Visit History";
        Size            = new Size(820, 620);
        MinimumSize     = new Size(820, 620);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        Font            = new Font("Segoe UI", 9.5f);

        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(123, 31, 162) };
        var lblTitle  = new Label { Text = "Animal Visit History", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
        pnlHeader.Controls.Add(lblTitle);

        // ── Search bar ────────────────────────────────────────────────────────
        var lblName = new Label { Text = "Animal Name:",  Location = new Point(20, 68), Size = new Size(100, 22), TextAlign = ContentAlignment.MiddleLeft };
        txtSearch   = new TextBox { Location = new Point(124, 66), Size = new Size(190, 28), Font = new Font("Segoe UI", 10) };

        var lblChip = new Label { Text = "Chip Number:",  Location = new Point(330, 68), Size = new Size(100, 22), TextAlign = ContentAlignment.MiddleLeft };
        txtChip     = new TextBox { Location = new Point(434, 66), Size = new Size(190, 28), Font = new Font("Segoe UI", 10) };

        btnSearch = new Button
        {
            Text      = "Search",
            Location  = new Point(640, 64),
            Size      = new Size(90, 30),
            BackColor = Color.FromArgb(123, 31, 162),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
        };
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.Click += BtnSearch_Click;

        txtSearch.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) BtnSearch_Click(null, EventArgs.Empty); };
        txtChip.KeyDown   += (_, e) => { if (e.KeyCode == Keys.Enter) BtnSearch_Click(null, EventArgs.Empty); };

        lblFound = new Label
        {
            Location  = new Point(20, 102),
            Size      = new Size(775, 20),
            ForeColor = Color.FromArgb(123, 31, 162),
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            Text      = string.Empty
        };

        // ── Visits grid ───────────────────────────────────────────────────────
        dgvVisits = new DataGridView
        {
            Location              = new Point(20, 126),
            Size                  = new Size(775, 230),
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect           = false,
            RowHeadersVisible     = false,
            BackgroundColor       = Color.White,
            Font                  = new Font("Segoe UI", 9.5f),
        };
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "#",           Width = 40  });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date & Time", Width = 155 });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reason",      Width = 230 });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Vet ID",      Width = 70  });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cost (₪)",    Width = 90  });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "",            Width = 90  });
        dgvVisits.DefaultCellStyle.SelectionBackColor = Color.FromArgb(209, 196, 233);
        dgvVisits.DefaultCellStyle.SelectionForeColor = Color.Black;
        dgvVisits.ColumnHeadersDefaultCellStyle.Font  = new Font("Segoe UI", 9, FontStyle.Bold);
        dgvVisits.SelectionChanged += DgvVisits_SelectionChanged;

        // ── Detail panel ──────────────────────────────────────────────────────
        var lblDetails = new Label { Text = "Visit Details", Location = new Point(20, 366), Size = new Size(200, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(123, 31, 162) };
        txtDetails = new TextBox
        {
            Location    = new Point(20, 390),
            Size        = new Size(775, 160),
            Multiline   = true,
            ReadOnly    = true,
            ScrollBars  = ScrollBars.Vertical,
            Font        = new Font("Consolas", 9.5f),
            BackColor   = Color.FromArgb(250, 245, 255),
            BorderStyle = BorderStyle.FixedSingle,
        };

        var btnClose = new Button { Text = "Close", Location = new Point(695, 562), Size = new Size(100, 32), FlatStyle = FlatStyle.Flat };
        btnClose.Click += (_, _) => Close();

        Controls.Add(pnlHeader);
        foreach (var c in new Control[] { lblName, txtSearch, lblChip, txtChip, btnSearch, lblFound, dgvVisits, lblDetails, txtDetails, btnClose })
            Controls.Add(c);
    }

    private void BtnSearch_Click(object? sender, EventArgs e)
    {
        lblFound.Text  = string.Empty;
        dgvVisits.Rows.Clear();
        txtDetails.Text = string.Empty;
        _visits.Clear();

        Animal? animal = null;

        var chip = txtChip.Text.Trim();
        var name = txtSearch.Text.Trim();

        if (!string.IsNullOrWhiteSpace(chip))
        {
            animal = _animalService.SearchByChipNumber(chip);
        }
        else if (!string.IsNullOrWhiteSpace(name))
        {
            var results = _animalService.SearchByName(name).ToList();
            if (results.Count == 1)
            {
                animal = results[0];
            }
            else if (results.Count > 1)
            {
                lblFound.Text     = $"Multiple animals match '{name}'. Please refine or use chip number.";
                lblFound.ForeColor = Color.OrangeRed;
                return;
            }
        }
        else
        {
            lblFound.Text      = "Enter an animal name or chip number to search.";
            lblFound.ForeColor = Color.OrangeRed;
            return;
        }

        if (animal == null)
        {
            lblFound.Text      = "No animal found.";
            lblFound.ForeColor = Color.OrangeRed;
            return;
        }

        var vaccineNote = _animalService.NeedsVaccination(animal) ? "  ⚠ Vaccination overdue!" : string.Empty;
        lblFound.Text      = $"Showing visits for: {animal.Name}  (Chip: {animal.ChipNumber}){vaccineNote}";
        lblFound.ForeColor = vaccineNote.Length > 0 ? Color.OrangeRed : Color.FromArgb(123, 31, 162);

        _visits = _visitService.GetVisitsByAnimal(animal.Id)
                               .OrderByDescending(v => v.VisitDateTime)
                               .ToList();
        PopulateGrid();
    }

    private void PopulateGrid()
    {
        dgvVisits.Rows.Clear();
        for (int i = 0; i < _visits.Count; i++)
        {
            var v = _visits[i];
            dgvVisits.Rows.Add(v.Id, v.VisitDateTime.ToString("dd/MM/yyyy  HH:mm"),
                v.Reason.Length > 55 ? v.Reason[..55] + "…" : v.Reason,
                v.VetEmployeeId, v.TotalCost.ToString("F2"),
                i == 0 ? "★ Most Recent" : string.Empty);

            if (i == 0)
            {
                dgvVisits.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(243, 229, 255);
                dgvVisits.Rows[i].DefaultCellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
        }

        if (_visits.Count == 0)
            txtDetails.Text = "No visits recorded for this animal.";
    }

    private void DgvVisits_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvVisits.SelectedRows.Count == 0 || dgvVisits.SelectedRows[0].Index >= _visits.Count) return;
        ShowDetail(_visits[dgvVisits.SelectedRows[0].Index]);
    }

    private void ShowDetail(Visit v)
    {
        var meds = v.Medicines.Count > 0
            ? string.Join("\n", v.Medicines.Select(m => $"    • {m.Name}  —  ₪{m.Price:F2}"))
            : "    (none)";
        txtDetails.Text =
            $"Visit #{v.Id}\n" +
            $"Date & Time : {v.VisitDateTime:dd/MM/yyyy  HH:mm}\n" +
            $"Reason      : {v.Reason}\n" +
            $"Diagnosis   : {(string.IsNullOrWhiteSpace(v.Diagnosis) ? "(none)" : v.Diagnosis)}\n" +
            $"Treating Vet: Employee #{v.VetEmployeeId}\n" +
            $"Medicines   :\n{meds}\n" +
            $"Total Cost  : ₪{v.TotalCost:F2}";
    }
}
