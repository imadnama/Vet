using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g3.Forms;

/// <summary>
/// Shows all visits to the clinic, most recent first.
/// </summary>
public class AllVisitsForm : Form
{
    private readonly IVisitService  _visitService;
    private readonly IAnimalService _animalService;

    private DataGridView dgvVisits  = null!;
    private TextBox      txtDetails = null!;
    private Label        lblCount   = null!;

    private List<Visit>  _visits  = new();
    private List<Animal> _animals = new();

    public AllVisitsForm(IVisitService visitService, IAnimalService animalService)
    {
        _visitService  = visitService;
        _animalService = animalService;
        InitializeComponent();
        LoadAll();
    }

    private void InitializeComponent()
    {
        Text            = "All Clinic Visits";
        Size            = new Size(860, 640);
        MinimumSize     = new Size(860, 640);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        Font            = new Font("Segoe UI", 9.5f);

        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(123, 31, 162) };
        var lblTitle  = new Label { Text = "All Clinic Visits", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
        pnlHeader.Controls.Add(lblTitle);

        lblCount = new Label
        {
            Location  = new Point(20, 62),
            Size      = new Size(600, 20),
            Font      = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(123, 31, 162),
            Text      = string.Empty
        };

        dgvVisits = new DataGridView
        {
            Location              = new Point(20, 88),
            Size                  = new Size(815, 260),
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
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date & Time", Width = 150 });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Animal",      Width = 150 });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reason",      Width = 200 });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Vet ID",      Width = 65  });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cost (₪)",    Width = 85  });
        dgvVisits.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "",            Width = 85  });
        dgvVisits.DefaultCellStyle.SelectionBackColor = Color.FromArgb(209, 196, 233);
        dgvVisits.DefaultCellStyle.SelectionForeColor = Color.Black;
        dgvVisits.ColumnHeadersDefaultCellStyle.Font  = new Font("Segoe UI", 9, FontStyle.Bold);
        dgvVisits.SelectionChanged += DgvVisits_SelectionChanged;

        var lblDetails = new Label { Text = "Visit Details", Location = new Point(20, 358), Size = new Size(200, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(123, 31, 162) };
        txtDetails = new TextBox
        {
            Location    = new Point(20, 382),
            Size        = new Size(815, 170),
            Multiline   = true,
            ReadOnly    = true,
            ScrollBars  = ScrollBars.Vertical,
            Font        = new Font("Consolas", 9.5f),
            BackColor   = Color.FromArgb(250, 245, 255),
            BorderStyle = BorderStyle.FixedSingle,
        };

        var btnClose = new Button { Text = "Close", Location = new Point(735, 566), Size = new Size(100, 32), FlatStyle = FlatStyle.Flat };
        btnClose.Click += (_, _) => Close();

        Controls.Add(pnlHeader);
        foreach (var c in new Control[] { lblCount, dgvVisits, lblDetails, txtDetails, btnClose })
            Controls.Add(c);
    }

    private void LoadAll()
    {
        _animals = _animalService.GetAll().ToList();
        _visits  = _visitService.GetAllVisits()
                                .OrderByDescending(v => v.VisitDateTime)
                                .ToList();

        lblCount.Text = $"Total visits: {_visits.Count}";

        for (int i = 0; i < _visits.Count; i++)
        {
            var v          = _visits[i];
            var animalName = _animals.FirstOrDefault(a => a.Id == v.AnimalId)?.Name ?? $"Animal #{v.AnimalId}";

            dgvVisits.Rows.Add(
                v.Id,
                v.VisitDateTime.ToString("dd/MM/yyyy  HH:mm"),
                animalName,
                v.Reason.Length > 50 ? v.Reason[..50] + "…" : v.Reason,
                v.VetEmployeeId,
                v.TotalCost.ToString("F2"),
                i == 0 ? "★ Most Recent" : string.Empty);

            if (i == 0)
            {
                dgvVisits.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(243, 229, 255);
                dgvVisits.Rows[i].DefaultCellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
        }
    }

    private void DgvVisits_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvVisits.SelectedRows.Count == 0 || dgvVisits.SelectedRows[0].Index >= _visits.Count) return;
        var v          = _visits[dgvVisits.SelectedRows[0].Index];
        var animalName = _animals.FirstOrDefault(a => a.Id == v.AnimalId)?.Name ?? $"Animal #{v.AnimalId}";
        var meds       = v.Medicines.Count > 0
            ? string.Join("\n", v.Medicines.Select(m => $"    • {m.Name}  —  ₪{m.Price:F2}"))
            : "    (none)";
        txtDetails.Text =
            $"Visit #{v.Id}\n" +
            $"Date & Time : {v.VisitDateTime:dd/MM/yyyy  HH:mm}\n" +
            $"Animal      : {animalName}\n" +
            $"Reason      : {v.Reason}\n" +
            $"Diagnosis   : {(string.IsNullOrWhiteSpace(v.Diagnosis) ? "(none)" : v.Diagnosis)}\n" +
            $"Treating Vet: Employee #{v.VetEmployeeId}\n" +
            $"Medicines   :\n{meds}\n" +
            $"Total Cost  : ₪{v.TotalCost:F2}";
    }
}
