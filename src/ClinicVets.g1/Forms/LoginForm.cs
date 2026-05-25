using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Login screen — beautiful redesign.
/// Soft mint background with decorative circles, paw logo, underline-style fields.
/// </summary>
public class LoginForm : Form
{
    private readonly IAuthService     _authService;
    private readonly Action<Employee> _onLoginSuccess;

    private TextBox _txtUsername     = null!;
    private TextBox _txtPassword     = null!;
    private Label   _lblError        = null!;
    private Panel   _pnlUserLine     = null!;  // underline indicator
    private Panel   _pnlPassLine     = null!;

    // Teal palette shared across the form
    private static readonly Color Teal      = Color.FromArgb(0, 150, 136);
    private static readonly Color TealDark  = Color.FromArgb(0, 121, 107);
    private static readonly Color TealLight = Color.FromArgb(178, 223, 219);
    private static readonly Color BgColor   = Color.FromArgb(240, 253, 252);

    public LoginForm(IAuthService authService, Action<Employee> onLoginSuccess)
    {
        _authService    = authService;
        _onLoginSuccess = onLoginSuccess;
        InitializeComponent();
    }

    // ── Layout ────────────────────────────────────────────────────────────────

    private void InitializeComponent()
    {
        Text            = "ClinicVets – Sign In";
        Size            = new Size(460, 560);
        MinimumSize     = new Size(460, 560);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        BackColor       = BgColor;
        DoubleBuffered  = true;
        Font            = new Font("Segoe UI", 9.5f);

        // The paw circle (y 38..128) is painted in OnPaintBackground.
        // All labelled controls sit below it.

        int cx   = 230; // horizontal center
        int fw   = 340; // field width
        int fx   = cx - fw / 2; // field X (60)

        // ── App name & subtitle ───────────────────────────────────────────────
        var lblTitle = MakeLabel("ClinicVets",
            new Font("Segoe UI", 25, FontStyle.Bold), TealDark,
            new Point(fx, 145), new Size(fw, 40), ContentAlignment.MiddleCenter);

        var lblSub1 = MakeLabel("Welcome Back",
            new Font("Segoe UI", 13), Color.FromArgb(80, 80, 80),
            new Point(fx, 188), new Size(fw, 26), ContentAlignment.MiddleCenter);

        var lblSub2 = MakeLabel("Veterinary Clinic Management System",
            new Font("Segoe UI", 8.5f), Color.FromArgb(150, 150, 150),
            new Point(fx, 215), new Size(fw, 20), ContentAlignment.MiddleCenter);

        // Thin divider
        var divider = new Panel
        {
            Location  = new Point(fx, 244),
            Size      = new Size(fw, 1),
            BackColor = TealLight,
        };

        // ── Username field ────────────────────────────────────────────────────
        var lblUser = MakeLabel("Username",
            new Font("Segoe UI", 8.5f, FontStyle.Bold), Color.FromArgb(80, 80, 80),
            new Point(fx, 258), new Size(fw, 18));

        _txtUsername = new TextBox
        {
            Location    = new Point(fx, 278),
            Size        = new Size(fw, 26),
            BorderStyle = BorderStyle.None,
            BackColor   = BgColor,
            Font        = new Font("Segoe UI", 12),
            ForeColor   = Color.FromArgb(33, 33, 33),
        };

        _pnlUserLine = new Panel
        {
            Location  = new Point(fx, 307),
            Size      = new Size(fw, 2),
            BackColor = TealLight,
        };

        // ── Password field ────────────────────────────────────────────────────
        var lblPass = MakeLabel("Password",
            new Font("Segoe UI", 8.5f, FontStyle.Bold), Color.FromArgb(80, 80, 80),
            new Point(fx, 323), new Size(fw, 18));

        _txtPassword = new TextBox
        {
            Location     = new Point(fx, 343),
            Size         = new Size(fw, 26),
            BorderStyle  = BorderStyle.None,
            BackColor    = BgColor,
            Font         = new Font("Segoe UI", 12),
            ForeColor    = Color.FromArgb(33, 33, 33),
            PasswordChar = '*',
        };

        _pnlPassLine = new Panel
        {
            Location  = new Point(fx, 372),
            Size      = new Size(fw, 2),
            BackColor = TealLight,
        };

        // ── Error label ───────────────────────────────────────────────────────
        _lblError = new Label
        {
            Location  = new Point(fx, 380),
            Size      = new Size(fw, 32),
            ForeColor = Color.FromArgb(198, 40, 40),
            Font      = new Font("Segoe UI", 8.5f),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };

        // ── Login button ──────────────────────────────────────────────────────
        var btnLogin = new Button
        {
            Text      = "SIGN IN",
            Location  = new Point(fx, 420),
            Size      = new Size(fw, 48),
            BackColor = Teal,
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 12, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        MakeRounded(btnLogin, 10);
        btnLogin.Click += btnLogin_Click;

        // Hover effect
        btnLogin.MouseEnter += (_, _) => btnLogin.BackColor = TealDark;
        btnLogin.MouseLeave += (_, _) => btnLogin.BackColor = Teal;

        // ── Register link ─────────────────────────────────────────────────────
        var lnkRegister = new LinkLabel
        {
            Text      = "Don't have an account?  Register New Employee",
            Location  = new Point(fx, 482),
            Size      = new Size(fw, 24),
            TextAlign = ContentAlignment.MiddleCenter,
            Font      = new Font("Segoe UI", 9),
            BackColor = Color.Transparent,
        };
        lnkRegister.LinkColor = Teal;
        lnkRegister.LinkClicked += lnkRegister_LinkClicked;

        // ── Focus underline animations ────────────────────────────────────────
        _txtUsername.Enter += (_, _) => _pnlUserLine.BackColor = Teal;
        _txtUsername.Leave += (_, _) => _pnlUserLine.BackColor = TealLight;
        _txtPassword.Enter += (_, _) => _pnlPassLine.BackColor = Teal;
        _txtPassword.Leave += (_, _) => _pnlPassLine.BackColor = TealLight;

        // ── Add to form ───────────────────────────────────────────────────────
        Controls.AddRange(new Control[]
        {
            lblTitle, lblSub1, lblSub2, divider,
            lblUser, _txtUsername, _pnlUserLine,
            lblPass, _txtPassword, _pnlPassLine,
            _lblError, btnLogin, lnkRegister,
        });

        AcceptButton = btnLogin;
    }

    // ── Background: gradient + decorative circles + paw ──────────────────────

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Soft gradient
        using (var bg = new LinearGradientBrush(ClientRectangle,
            Color.FromArgb(220, 248, 246),
            Color.FromArgb(245, 252, 252),
            90f))
        {
            g.FillRectangle(bg, ClientRectangle);
        }

        // Decorative circles (very subtle, like the reference photos)
        using (var d1 = new SolidBrush(Color.FromArgb(22, 0, 150, 136)))
        using (var d2 = new SolidBrush(Color.FromArgb(12, 0, 150, 136)))
        {
            g.FillEllipse(d1, -110, -110, 300, 300);            // top-left
            g.FillEllipse(d1, Width - 170, Height - 170, 300, 300); // bottom-right
            g.FillEllipse(d2, Width - 200, -70,  260, 260);     // top-right
            g.FillEllipse(d2, -60, Height - 160, 240, 240);     // bottom-left
        }

        // Teal circle behind paw
        float cx = Width / 2f, cy = 83f;
        using (var circle = new SolidBrush(Color.FromArgb(0, 137, 123)))
        {
            g.FillEllipse(circle, cx - 45, cy - 45, 90, 90);
        }
        // Subtle ring around it
        using (var ring = new Pen(TealLight, 3))
        {
            g.DrawEllipse(ring, cx - 54, cy - 54, 108, 108);
        }

        // White paw inside the teal circle
        DrawPaw(g, cx, cy, 54f, Color.White);
    }

    private static void DrawPaw(Graphics g, float cx, float cy, float s, Color c)
    {
        using var b = new SolidBrush(c);
        // Main palm pad
        g.FillEllipse(b, cx - s * 0.38f, cy - s * 0.06f, s * 0.76f, s * 0.62f);
        // Four toe pads
        float tw = s * 0.23f, th = s * 0.25f;
        float[,] t =
        {
            { cx - s * 0.44f, cy - s * 0.43f },
            { cx - s * 0.16f, cy - s * 0.60f },
            { cx + s * 0.16f, cy - s * 0.60f },
            { cx + s * 0.44f, cy - s * 0.43f },
        };
        for (int i = 0; i < 4; i++)
            g.FillEllipse(b, t[i, 0] - tw / 2, t[i, 1] - th / 2, tw, th);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Label MakeLabel(string text, Font font, Color fore,
        Point loc, Size size,
        ContentAlignment align = ContentAlignment.MiddleLeft) => new()
    {
        Text      = text,
        Font      = font,
        ForeColor = fore,
        Location  = loc,
        Size      = size,
        TextAlign = align,
        BackColor = Color.Transparent,
    };

    private static void MakeRounded(Control ctrl, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(0,               0,                d, d, 180, 90);
        path.AddArc(ctrl.Width - d,  0,                d, d, 270, 90);
        path.AddArc(ctrl.Width - d,  ctrl.Height - d,  d, d,   0, 90);
        path.AddArc(0,               ctrl.Height - d,  d, d,  90, 90);
        path.CloseFigure();
        ctrl.Region = new Region(path);
    }

    // ── Event handlers ────────────────────────────────────────────────────────

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        _lblError.Text = string.Empty;

        string username = _txtUsername.Text.Trim();
        string password = _txtPassword.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _lblError.Text = "Please enter both username and password.";
            return;
        }

        try
        {
            if (_authService.Login(username, password, out var employee) && employee != null)
            {
                _onLoginSuccess(employee);
            }
            else
            {
                _lblError.Text = "Invalid username or password. Please try again.";
                _txtPassword.Clear();
                _txtPassword.Focus();
            }
        }
        catch (Exception ex)
        {
            _lblError.Text = $"Login error: {ex.Message}";
        }
    }

    private void lnkRegister_LinkClicked(object? sender, EventArgs e)
    {
        using var form = new RegisterEmployeeForm(_authService);
        form.ShowDialog(this);
    }
}
