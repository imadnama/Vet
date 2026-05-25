using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using ClinicVets.g1.Validation;

namespace ClinicVets.g1.Forms;

public class RegisterEmployeeForm : Form
{
    private readonly IAuthService _authService;

    private TextBox     _txtFullName        = null!;
    private TextBox     _txtUsername        = null!;
    private TextBox     _txtPassword        = null!;
    private TextBox     _txtConfirmPassword = null!;
    private TextBox     _txtEmployeeNumber  = null!;
    private TextBox     _txtEmail           = null!;
    private TextBox     _txtNationalId      = null!;
    private RadioButton _rbVeterinarian     = null!;
    private RadioButton _rbSecretary        = null!;

    private Label _errFullName        = null!;
    private Label _errUsername        = null!;
    private Label _errPassword        = null!;
    private Label _errConfirmPassword = null!;
    private Label _errEmployeeNumber  = null!;
    private Label _errEmail           = null!;
    private Label _errNationalId      = null!;
    private Label _lblGeneralError    = null!;

    public RegisterEmployeeForm(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "ClinicVets – Register Employee";
        Size            = new Size(480, 800);
        MinimumSize     = new Size(480, 800);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = Color.White;
        Font            = new Font("Segoe UI", 9.5f);

        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(21, 101, 192) };
        var lblTitle  = new Label { Text = "Register New Employee", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
        pnlHeader.Controls.Add(lblTitle);

        const int x   = 30;
        const int w   = 414;
        const int lh  = 18;
        const int th  = 28;
        const int gap = 72;
        int y = 72;

        (Label fieldLbl, TextBox txt, Label errLbl) MakeField(string labelText, bool isPassword = false)
        {
            var fieldLabel = new Label
            {
                Text     = labelText,
                Font     = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(x, y),
                Size     = new Size(w, lh)
            };
            var textBox = new TextBox
            {
                Location     = new Point(x, y + lh + 2),
                Size         = new Size(w, th),
                Font         = new Font("Segoe UI", 10),
                PasswordChar = isPassword ? '*' : '\0'
            };
            var errLabel = new Label
            {
                Location  = new Point(x, y + lh + 2 + th + 2),
                Size      = new Size(w, 16),
                ForeColor = Color.FromArgb(198, 40, 40),
                Font      = new Font("Segoe UI", 8),
                Text      = string.Empty
            };
            y += gap;
            return (fieldLabel, textBox, errLabel);
        }

        var (lblFN,   txtFN,   errFN)   = MakeField("Full Name");
        var (lblUser, txtUser, errUser) = MakeField("Username  (6–8 chars, max 2 digits)");
        var (lblPwd,  txtPwd,  errPwd)  = MakeField("Password  (8–10 chars, letter + digit + special: ! # $ ,)", true);
        var (lblCpwd, txtCpwd, errCpwd) = MakeField("Confirm Password", true);
        var (lblEmp,  txtEmp,  errEmp)  = MakeField("Employee Number  (4 digits)");
        var (lblMail, txtMail, errMail) = MakeField("Email");
        var (lblNat,  txtNat,  errNat)  = MakeField("National ID  (9 digits)");

        _txtFullName        = txtFN;
        _txtUsername        = txtUser;
        _txtPassword        = txtPwd;
        _txtConfirmPassword = txtCpwd;
        _txtEmployeeNumber  = txtEmp;
        _txtEmail           = txtMail;
        _txtNationalId      = txtNat;

        _errFullName        = errFN;
        _errUsername        = errUser;
        _errPassword        = errPwd;
        _errConfirmPassword = errCpwd;
        _errEmployeeNumber  = errEmp;
        _errEmail           = errMail;
        _errNationalId      = errNat;

        var lblRole     = new Label       { Text = "Role", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(x, y), Size = new Size(w, lh) };
        _rbVeterinarian = new RadioButton { Text = "Veterinarian", Location = new Point(x, y + lh + 4), Font = new Font("Segoe UI", 10), Checked = true };
        _rbSecretary    = new RadioButton { Text = "Secretary",    Location = new Point(x + 130, y + lh + 4), Font = new Font("Segoe UI", 10) };
        y += 55;

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

        _lblGeneralError = new Label
        {
            Location  = new Point(x, y),
            Size      = new Size(w, 20),
            ForeColor = Color.FromArgb(198, 40, 40),
            Font      = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        Controls.Add(pnlHeader);
        foreach (var ctrl in new Control[]
        {
            lblFN,   txtFN,   errFN,
            lblUser, txtUser, errUser,
            lblPwd,  txtPwd,  errPwd,
            lblCpwd, txtCpwd, errCpwd,
            lblEmp,  txtEmp,  errEmp,
            lblMail, txtMail, errMail,
            lblNat,  txtNat,  errNat,
            lblRole, _rbVeterinarian, _rbSecretary,
            btnRegister, _lblGeneralError,
        })
            Controls.Add(ctrl);

        AcceptButton = btnRegister;
    }

    private void btnRegister_Click(object? sender, EventArgs e)
    {
        ClearAllErrors();
        bool valid = true;

        if (string.IsNullOrWhiteSpace(_txtFullName.Text))
        {
            _errFullName.Text = "Full name is required.";
            valid = false;
        }

        if (!EmployeeValidator.ValidateUsername(_txtUsername.Text.Trim(), out var errUser))
        {
            _errUsername.Text = errUser;
            valid = false;
        }

        if (!EmployeeValidator.ValidatePassword(_txtPassword.Text, out var errPwd))
        {
            _errPassword.Text = errPwd;
            valid = false;
        }
        else if (_txtPassword.Text != _txtConfirmPassword.Text)
        {
            _errConfirmPassword.Text = "Passwords do not match.";
            valid = false;
        }

        if (!EmployeeValidator.ValidateEmployeeNumber(_txtEmployeeNumber.Text.Trim(), out var errEmp))
        {
            _errEmployeeNumber.Text = errEmp;
            valid = false;
        }

        if (!EmployeeValidator.ValidateEmail(_txtEmail.Text.Trim(), out var errMail))
        {
            _errEmail.Text = errMail;
            valid = false;
        }

        if (!EmployeeValidator.ValidateNationalId(_txtNationalId.Text.Trim(), out var errNat))
        {
            _errNationalId.Text = errNat;
            valid = false;
        }

        if (!valid) return;

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
            _lblGeneralError.Text = "Username or Employee Number already exists.";
        }
    }

    private void ClearAllErrors()
    {
        _errFullName.Text        = string.Empty;
        _errUsername.Text        = string.Empty;
        _errPassword.Text        = string.Empty;
        _errConfirmPassword.Text = string.Empty;
        _errEmployeeNumber.Text  = string.Empty;
        _errEmail.Text           = string.Empty;
        _errNationalId.Text      = string.Empty;
        _lblGeneralError.Text    = string.Empty;
    }
}
