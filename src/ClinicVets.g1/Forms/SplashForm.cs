using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace ClinicVets.g1.Forms;

/// <summary>
/// Animated splash screen shown before the login form.
/// Flow: Fade-in (600 ms) → hold with pulsing dots (1.8 s) → fade-out (500 ms).
/// </summary>
public class SplashForm : Form
{
    private enum Phase { FadeIn, Hold, FadeOut }

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 22 };
    private Phase  _phase     = Phase.FadeIn;
    private double _opacity   = 0.0;
    private int    _holdTick  = 0;
    private int    _dotTick   = 0;

    public SplashForm()
    {
        Text            = "ClinicVets";
        Size            = new Size(520, 380);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.None;
        BackColor       = Color.FromArgb(0, 77, 64);
        Opacity         = 0;
        DoubleBuffered  = true;

        _timer.Tick += OnTick;
        _timer.Start();
    }

    // ── Animation state machine ───────────────────────────────────────────────

    private void OnTick(object? sender, EventArgs e)
    {
        switch (_phase)
        {
            case Phase.FadeIn:
                _opacity += 0.038;          // ~600 ms to reach 1
                if (_opacity >= 1.0) { _opacity = 1.0; _phase = Phase.Hold; }
                Opacity = _opacity;
                break;

            case Phase.Hold:
                _holdTick++;
                _dotTick  = (_dotTick + 1) % 90;
                if (_holdTick >= 82) _phase = Phase.FadeOut;   // ~1.8 s
                Invalidate();
                break;

            case Phase.FadeOut:
                _opacity -= 0.044;          // ~500 ms
                if (_opacity <= 0) { _timer.Stop(); Close(); return; }
                Opacity = _opacity;
                break;
        }
    }

    // ── Custom painting ───────────────────────────────────────────────────────

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode        = SmoothingMode.AntiAlias;
        g.TextRenderingHint    = TextRenderingHint.AntiAliasGridFit;
        g.InterpolationMode    = InterpolationMode.HighQualityBicubic;

        float w = Width, h = Height;

        // ── Gradient background ──────────────────────────────────────────────
        using (var bg = new LinearGradientBrush(ClientRectangle,
            Color.FromArgb(0,  60, 50),
            Color.FromArgb(0, 105, 92),
            LinearGradientMode.ForwardDiagonal))
        {
            g.FillRectangle(bg, ClientRectangle);
        }

        // ── Large decorative translucent circles ─────────────────────────────
        using (var decoA = new SolidBrush(Color.FromArgb(18, 255, 255, 255)))
        using (var decoB = new SolidBrush(Color.FromArgb(10, 255, 255, 255)))
        {
            g.FillEllipse(decoA, -100, -100, 280, 280);
            g.FillEllipse(decoA, w - 160, h - 160, 280, 280);
            g.FillEllipse(decoB, w - 200, -80, 300, 300);
        }

        // ── Paw logo ─────────────────────────────────────────────────────────
        float cx = w / 2f, cy = h / 2f - 45f;
        DrawPaw(g, cx, cy, 80f, Color.White);

        // ── Title ─────────────────────────────────────────────────────────────
        using var titleFont = new Font("Segoe UI", 28, FontStyle.Bold, GraphicsUnit.Pixel);
        using var titleFont2 = new Font("Segoe UI", 28 * 96 / 72, FontStyle.Bold);
        DrawCenteredString(g, "ClinicVets",
            new Font("Segoe UI", 26, FontStyle.Bold),
            Color.White, cx, cy + 62);

        // ── Subtitle ──────────────────────────────────────────────────────────
        DrawCenteredString(g, "Veterinary Clinic Management System",
            new Font("Segoe UI", 10.5f),
            Color.FromArgb(170, 255, 255, 255), cx, cy + 100);

        // ── Animated loading dots ─────────────────────────────────────────────
        DrawLoadingDots(g, cx, h - 52f);
    }

    private static void DrawPaw(Graphics g, float cx, float cy, float s, Color c)
    {
        using var b = new SolidBrush(c);

        // Main palm pad
        g.FillEllipse(b, cx - s * 0.38f, cy - s * 0.06f, s * 0.76f, s * 0.62f);

        // Four toe pads
        float tw = s * 0.23f, th = s * 0.25f;
        float[,] toes =
        {
            { cx - s * 0.44f, cy - s * 0.43f },
            { cx - s * 0.16f, cy - s * 0.60f },
            { cx + s * 0.16f, cy - s * 0.60f },
            { cx + s * 0.44f, cy - s * 0.43f },
        };
        for (int i = 0; i < 4; i++)
            g.FillEllipse(b, toes[i, 0] - tw / 2, toes[i, 1] - th / 2, tw, th);
    }

    private void DrawLoadingDots(Graphics g, float cx, float cy)
    {
        const float spacing = 24f;
        const float baseR   = 5f;
        const float TAU     = (float)(Math.PI * 2);

        for (int i = 0; i < 3; i++)
        {
            float phase = (_dotTick / 90f * TAU) + i * TAU / 3f;
            float pulse = (float)(Math.Sin(phase) * 0.5 + 0.5); // 0..1
            float r     = baseR + pulse * 3f;
            int   alpha = 70 + (int)(pulse * 185);

            using var db = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255));
            float x = cx + (i - 1) * spacing;
            g.FillEllipse(db, x - r, cy - r, r * 2, r * 2);
        }
    }

    private static void DrawCenteredString(Graphics g, string text, Font font, Color color, float cx, float y)
    {
        using var brush = new SolidBrush(color);
        var sz = g.MeasureString(text, font);
        g.DrawString(text, font, brush, cx - sz.Width / 2f, y);
    }
}
