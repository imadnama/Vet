using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Screen 3 — Register a new customer (animal owner).
/// Accessible only when the current user is a Secretary.
/// Role enforcement is applied in MainForm before this form is opened.
/// </summary>
public class CustomerRegistrationForm : Form
{
    private readonly ICustomerService _customerService;

    private TextBox _txtFullName   = null!;
    private TextBox _txtNationalId = null!;
    private TextBox _txtPhone      = null!;
    private TextBox _txtEmail      = null!;
    private Label   _lblError      = null!;

    public CustomerRegistrationForm(ICustomerService customerService)
    {
        _customerService = customerService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // ── Form properties ──────────────────────────────────────────────────
        Text            = "ClinicVets – Customer Registration";
        Size            = new Size(480, 470);
        MinimumSize     = new Size(480, 470);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = Color.White;
        Font            = new Font("Segoe UI", 9.5f);

        // ── Header ───────────────────────────────────────────────────────────
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(46, 125, 50) };
        var lblTitle  = new Label { Text = "Register New Customer", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
        pnlHeader.Controls.Add(lblTitle);

        // ── Fields ───────────────────────────────────────────────────────────
        int x   = 30;
        int w   = 414;
        int lh  = 20;
        int th  = 30;
        int gap = 55;
        int y   = 72;

        (Label lbl, TextBox txt) MakeField(string labelText)
        {
            var lbl = new Label { Text = labelText, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(x, y), Size = new Size(w, lh) };
            var txt = new TextBox { Location = new Point(x, y + lh + 2), Size = new Size(w, th), Font = new Font("Segoe UI", 10) };
            y += gap;
            return (lbl, txt);
        }

        var (lblFN,   txtFN)   = MakeField("Full Name  (letters only)");
        var (lblNat,  txtNat)  = MakeField("National ID  (9 digits)");
        var (lblPh,   txtPh)   = MakeField("Phone  (e.g. 050-1234567)");
        var (lblMail, txtMail) = MakeField("Email");

        _txtFullName   = txtFN;
        _txtNationalId = txtNat;
        _txtPhone      = txtPh;
        _txtEmail      = txtMail;

        // ── Save button ──────────────────────────────────────────────────────
        var btnSave = new Button
        {
            Text      = "SAVE CUSTOMER",
            Location  = new Point(x, y),
            Size      = new Size(w, 44),
            BackColor = Color.FromArgb(46, 125, 50),
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 12, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += btnSave_Click;
        y += 50;

        // ── Error label ──────────────────────────────────────────────────────
        _lblError = new Label
        {
            Location  = new Point(x, y),
            Size      = new Size(w, 36),
            ForeColor = Color.FromArgb(198, 40, 40),
            Font      = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        Controls.Add(pnlHeader);
        foreach (var ctrl in new Control[]
        {
            lblFN, txtFN, lblNat, txtNat, lblPh, txtPh, lblMail, txtMail,
            btnSave, _lblError,
        })
        {
            Controls.Add(ctrl);
        }

        AcceptButton = btnSave;
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        _lblError.Text = string.Empty;

        var customer = new Customer
        {
            FullName   = _txtFullName.Text.Trim(),
            NationalId = _txtNationalId.Text.Trim(),
            Phone      = _txtPhone.Text.Trim(),
            Email      = _txtEmail.Text.Trim(),
        };

        if (!_customerService.RegisterCustomer(customer, out var error))
        {
            _lblError.Text = error;
            return;
        }

        MessageBox.Show(
            $"Customer '{customer.FullName}' registered successfully!",
            "Success",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        ClearFields();
    }

    private void ClearFields()
    {
        _txtFullName.Clear();
        _txtNationalId.Clear();
        _txtPhone.Clear();
        _txtEmail.Clear();
        _txtFullName.Focus();
    }
}
