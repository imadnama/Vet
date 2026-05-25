using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Validation;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Screen 2 — Register a new clinic employee.
/// Fields: FullName, Username, Password, ConfirmPassword,
///         EmployeeNumber, Email, NationalId, Role.
/// </summary>
public class RegisterEmployeeForm : Form
{
    private readonly IAuthService _authService;

    private TextBox      _txtFullName        = null!;
    private TextBox      _txtUsername        = null!;
    private TextBox      _txtPassword        = null!;
    private TextBox      _txtConfirmPassword = null!;
    private TextBox      _txtEmployeeNumber  = null!;
    private TextBox      _txtEmail           = null!;
    private TextBox      _txtNationalId      = null!;
    private RadioButton  _rbVeterinarian     = null!;
    private RadioButton  _rbSecretary        = null!;
    private Label        _lblError           = null!;

    public RegisterEmployeeForm(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // ── Form properties ──────────────────────────────────────────────────
        Text            = "ClinicVets – Register Employee";
        Size            = new Size(480, 680);
        MinimumSize     = new Size(480, 680);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = Color.White;
        Font            = new Font("Segoe UI", 9.5f);

        // ── Header ───────────────────────────────────────────────────────────
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(21, 101, 192) };
        var lblTitle  = new Label { Text = "Register New Employee", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
        pnlHeader.Controls.Add(lblTitle);

        // ── Field helper ─────────────────────────────────────────────────────
        int x     = 30;
        int w     = 414;
        int lh    = 20;
        int th    = 30;
        int gap   = 55; // spacing between field groups
        int y     = 72;

        (Label lbl, TextBox txt) MakeField(string labelText, bool isPassword = false)
        {
            var lbl = new Label { Text = labelText, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(x, y), Size = new Size(w, lh) };
            var txt = new TextBox { Location = new Point(x, y + lh + 2), Size = new Size(w, th), Font = new Font("Segoe UI", 10) };
            if (isPassword) txt.PasswordChar = '*';
            y += gap;
            return (lbl, txt);
        }

        var (lblFN,   txtFN)   = MakeField("Full Name");
        var (lblUser, txtUser) = MakeField("Username  (6–8 chars, max 2 digits)");
        var (lblPwd,  txtPwd)  = MakeField("Password  (8–10 chars, letter + digit + special: ! # $ ,)", true);
        var (lblCpwd, txtCpwd) = MakeField("Confirm Password", true);
        var (lblEmp,  txtEmp)  = MakeField("Employee Number  (4 digits)");
        var (lblMail, txtMail) = MakeField("Email");
        var (lblNat,  txtNat)  = MakeField("National ID  (9 digits)");

        _txtFullName        = txtFN;
        _txtUsername        = txtUser;
        _txtPassword        = txtPwd;
        _txtConfirmPassword = txtCpwd;
        _txtEmployeeNumber  = txtEmp;
        _txtEmail           = txtMail;
        _txtNationalId      = txtNat;

        // ── Role radios ──────────────────────────────────────────────────────
        var lblRole = new Label { Text = "Role", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(x, y), Size = new Size(w, lh) };
        _rbVeterinarian = new RadioButton { Text = "Veterinarian", Location = new Point(x, y + lh + 4), Font = new Font("Segoe UI", 10), Checked = true };
        _rbSecretary    = new RadioButton { Text = "Secretary",    Location = new Point(x + 130, y + lh + 4), Font = new Font("Segoe UI", 10) };
        y += gap;

        // ── Register button ──────────────────────────────────────────────────
        var btnRegister = new Button
        {
            Text      = "REGISTER EMPLOYEE",
            Location  = new Point(x, y),
            Size      = new Size(w, 44),
            BackColor = Color.FromArgb(21, 101, 192),
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 12, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
        };
        btnRegister.FlatAppearance.BorderSize = 0;
        btnRegister.Click += btnRegister_Click;
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

        // ── Add all controls ─────────────────────────────────────────────────
        Controls.Add(pnlHeader);
        foreach (var ctrl in new Control[]
        {
            lblFN, txtFN, lblUser, txtUser, lblPwd, txtPwd, lblCpwd, txtCpwd,
            lblEmp, txtEmp, lblMail, txtMail, lblNat, txtNat,
            lblRole, _rbVeterinarian, _rbSecretary,
            btnRegister, _lblError,
        })
        {
            Controls.Add(ctrl);
        }

        AcceptButton = btnRegister;
    }

    private void btnRegister_Click(object? sender, EventArgs e)
    {
        _lblError.Text = string.Empty;

        // Full name (not covered by EmployeeValidator — check manually)
        if (string.IsNullOrWhiteSpace(_txtFullName.Text.Trim()))
        {
            _lblError.Text = "Full name is required.";
            return;
        }

        if (!EmployeeValidator.ValidateUsername(_txtUsername.Text.Trim(), out var err))  { _lblError.Text = err; return; }
        if (!EmployeeValidator.ValidatePassword(_txtPassword.Text, out err))             { _lblError.Text = err; return; }

        if (_txtPassword.Text != _txtConfirmPassword.Text)
        {
            _lblError.Text = "Passwords do not match.";
            return;
        }

        if (!EmployeeValidator.ValidateEmployeeNumber(_txtEmployeeNumber.Text.Trim(), out err)) { _lblError.Text = err; return; }
        if (!EmployeeValidator.ValidateEmail(_txtEmail.Text.Trim(), out err))                   { _lblError.Text = err; return; }
        if (!EmployeeValidator.ValidateNationalId(_txtNationalId.Text.Trim(), out err))         { _lblError.Text = err; return; }

        var employee = new Employee
        {
            FullName       = _txtFullName.Text.Trim(),
            Username       = _txtUsername.Text.Trim(),
            EmployeeNumber = _txtEmployeeNumber.Text.Trim(),
            Email          = _txtEmail.Text.Trim(),
            NationalId     = _txtNationalId.Text.Trim(),
            Role           = _rbVeterinarian.Checked ? Role.Veterinarian : Role.Secretary,
        };

        if (_authService.RegisterEmployee(employee, _txtPassword.Text))
        {
            MessageBox.Show(
                $"Employee '{employee.Username}' registered successfully!\nRole: {employee.Role}",
                "Registration Successful",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Close();
        }
        else
        {
            _lblError.Text = "Registration failed. Username or Employee Number already exists.";
        }
    }
}
