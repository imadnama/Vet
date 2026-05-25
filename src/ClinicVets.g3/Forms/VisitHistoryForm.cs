using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g3.Forms;

public class VisitHistoryForm : Form
{
    private readonly IVisitService  _visitService;
    private readonly IAnimalService _animalService;

    private ComboBox      cmbAnimal    = null!;
    private DataGridView  dgvVisits    = null!;
    private Label         lblAnimalHdr = null!;
    private Label         lblDetails   = null!;
    private TextBox       txtDetails   = null!;

    private List<Animal> _animals = new();
    private List<Visit>  _visits  = new();

    public VisitHistoryForm(IVisitService visitService, IAnimalService animalService)
    {
        _visitService  = visitService;
        _animalService = animalService;
        InitializeComponent();
        LoadAnimals();
    }

    private void InitializeComponent()
    {
        Text            = "Visit History";
        Size            = new Size(820, 620);
        MinimumSize     = new Size(820, 620);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        Font            = new Font("Segoe UI", 9.5f);

        // ── Header ───────────────────────────────────────────────────────────
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(123, 31, 162) };
        var lblTitle  = new Label { Text = "Visit History", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
        pnlHeader.Controls.Add(lblTitle);

        // ── Animal selector ───────────────────────────────────────────────────
        var lblSelect = new Label { Text = "Select Animal:", Location = new Point(20, 66), Size = new Size(110, 22), TextAlign = ContentAlignment.MiddleLeft };
        cmbAnimal = new ComboBox
        {
            Location      = new Point(134, 64),
            Size          = new Size(340, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 10)
        };
        cmbAnimal.SelectedIndexChanged += CmbAnimal_SelectedIndexChanged;

        lblAnimalHdr = new Label
        {
            Location  = new Point(490, 64),
            Size      = new Size(300, 22),
            ForeColor = Color.FromArgb(123, 31, 162),
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            Text      = string.Empty
        };

        // ── Visits grid ───────────────────────────────────────────────────────
        dgvVisits = new DataGridView
        {
            Location              = new Point(20, 100),
            Size                  = new Size(775, 240),
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect           = false,
            RowHeadersVisible     = false,
            BackgroundColor       = Color.White,
            BorderStyle           = BorderStyle.Fixed3D,
            Font                  = new Font("Segoe UI", 9.5f),
        };
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id",       HeaderText = "#",           Width = 40,  ReadOnly = true });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { Name = "DateTime", HeaderText = "Date & Time", Width = 155, ReadOnly = true });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { Name = "Reason",   HeaderText = "Reason",      Width = 220, ReadOnly = true });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vet",      HeaderText = "Vet ID",      Width = 70,  ReadOnly = true });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cost",     HeaderText = "Cost (₪)",    Width = 90,  ReadOnly = true });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tag",      HeaderText = "",            Width = 90,  ReadOnly = true });

        dgvVisits.DefaultCellStyle.SelectionBackColor = Color.FromArgb(209, 196, 233);
        dgvVisits.DefaultCellStyle.SelectionForeColor = Color.Black;
        dgvVisits.ColumnHeadersDefaultCellStyle.Font  = new Font("Segoe UI", 9, FontStyle.Bold);

        dgvVisits.SelectionChanged += DgvVisits_SelectionChanged;

        // ── Detail panel ──────────────────────────────────────────────────────
        lblDetails = new Label
        {
            Text     = "Visit Details",
            Location = new Point(20, 350),
            Size     = new Size(200, 20),
            Font     = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(123, 31, 162),
        };

        txtDetails = new TextBox
        {
            Location   = new Point(20, 374),
            Size       = new Size(775, 170),
            Multiline  = true,
            ReadOnly   = true,
            ScrollBars = ScrollBars.Vertical,
            Font       = new Font("Consolas", 9.5f),
            BackColor  = Color.FromArgb(250, 245, 255),
            BorderStyle = BorderStyle.FixedSingle,
        };

        var btnClose = new Button
        {
            Text      = "Close",
            Location  = new Point(695, 555),
            Size      = new Size(100, 32),
            FlatStyle = FlatStyle.Flat,
        };
        btnClose.Click += (_, _) => Close();

        Controls.Add(pnlHeader);
        Controls.Add(lblSelect);
        Controls.Add(cmbAnimal);
        Controls.Add(lblAnimalHdr);
        Controls.Add(dgvVisits);
        Controls.Add(lblDetails);
        Controls.Add(txtDetails);
        Controls.Add(btnClose);
    }

    private void LoadAnimals()
    {
        _animals = _animalService.GetAll().ToList();
        cmbAnimal.Items.Clear();
        foreach (var a in _animals)
            cmbAnimal.Items.Add($"{a.Name}  (Chip: {a.ChipNumber})");
    }

    private void CmbAnimal_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbAnimal.SelectedIndex < 0) return;
        var animal = _animals[cmbAnimal.SelectedIndex];

        lblAnimalHdr.Text = _animalService.NeedsVaccination(animal)
            ? "⚠ Vaccination overdue!"
            : string.Empty;

        _visits = _visitService.GetVisitsByAnimal(animal.Id)
                               .OrderByDescending(v => v.VisitDateTime)
                               .ToList();
        RefreshGrid();
        txtDetails.Text = string.Empty;
    }

    private void RefreshGrid()
    {
        dgvVisits.Rows.Clear();

        for (int i = 0; i < _visits.Count; i++)
        {
            var v   = _visits[i];
            var tag = i == 0 ? "★ Most Recent" : string.Empty;
            dgvVisits.Rows.Add(
                v.Id,
                v.VisitDateTime.ToString("dd/MM/yyyy  HH:mm"),
                v.Reason.Length > 50 ? v.Reason[..50] + "…" : v.Reason,
                v.VetEmployeeId,
                v.TotalCost.ToString("F2"),
                tag);

            if (i == 0)
            {
                var row = dgvVisits.Rows[i];
                row.DefaultCellStyle.BackColor = Color.FromArgb(243, 229, 255);
                row.DefaultCellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
        }

        if (_visits.Count == 0)
            txtDetails.Text = "No visits recorded for this animal.";
    }

    private void DgvVisits_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvVisits.SelectedRows.Count == 0) return;
        int idx = dgvVisits.SelectedRows[0].Index;
        if (idx < 0 || idx >= _visits.Count) return;

        var v = _visits[idx];
        var medicines = v.Medicines.Count > 0
            ? string.Join("\n", v.Medicines.Select(m => $"    • {m.Name}  —  ₪{m.Price:F2}"))
            : "    (none)";

        txtDetails.Text =
            $"Visit #{v.Id}\n" +
            $"Date & Time : {v.VisitDateTime:dd/MM/yyyy  HH:mm}\n" +
            $"Reason      : {v.Reason}\n" +
            $"Diagnosis   : {(string.IsNullOrWhiteSpace(v.Diagnosis) ? "(none)" : v.Diagnosis)}\n" +
            $"Treating Vet: Employee #{v.VetEmployeeId}\n" +
            $"Medicines   :\n{medicines}\n" +
            $"Total Cost  : ₪{v.TotalCost:F2}";
    }
}
