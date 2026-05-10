using Google.Apis.Gmail.v1;
using System.Drawing;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class EmailRow : UserControl
    {
        public EmailModel? Email { get; private set; }
        private GmailService? _gmailService;
        public Action? OnDraftClicked;
        public bool IsAnsweredRow { get; set; } = false;
        private readonly Color normalFill = Color.FromArgb(26, 28, 46);
        private readonly Color hoverFill = Color.FromArgb(17, 18, 30);
        private readonly Color normalBorder = Color.FromArgb(39, 40, 64);
        private readonly Color hoverBorder = Color.FromArgb(43, 40, 89);

        private bool _forceRead = false;

        
        public EmailRow()
        {
            InitializeComponent();
            AttachEvents(this);
        }

        public bool ForceReadAppearance
        {
            set
            {
                _forceRead = value;
                if (value)
                {
                    guna2CirclePictureBox2.Visible = false;
                    guna2HtmlLabel1.Font = new Font(guna2HtmlLabel1.Font, FontStyle.Regular);
                    guna2HtmlLabel3.Location = new Point(guna2HtmlLabel3.Location.X, guna2HtmlLabel3.Location.Y);
                }
            }
        }

        public void SetEmail(EmailModel email, GmailService service)
        {
            Email = email;
            _gmailService = service;

            guna2HtmlLabel1.Text = email.FromName;
            guna2HtmlLabel2.Text = email.Snippet;
            guna2HtmlLabel3.Text = email.Date;

            // Unread dot visibility
            guna2CirclePictureBox2.Visible = !email.IsRead;

            if (!email.IsRead)
            {
                // Bold sender name when unread
                guna2HtmlLabel1.Font = new Font(guna2HtmlLabel1.Font, FontStyle.Bold);
            }
            else
            {
                // Already read — move time to the right just like when clicked
                guna2HtmlLabel3.Location = new Point(guna2HtmlLabel3.Location.X + 20, guna2HtmlLabel3.Location.Y);
            }

            SetSenderAvatar(email.FromName, email.FromEmail);
        }

        private void AttachEvents(Control control)
        {
            control.MouseEnter += OnHoverEnter;
            control.MouseLeave += OnHoverLeave;

            // Only attach click to the top-level UserControl and rowPanel, not every child
            if (control == this || control == rowPanel)
                control.MouseClick += OnRowClick;

            foreach (Control child in control.Controls)
                AttachEvents(child);
        }

        private void OnHoverEnter(object? sender, EventArgs e)
        {
            rowPanel.FillColor = hoverFill;
            rowPanel.BorderColor = hoverBorder;
            this.Cursor = Cursors.Hand;
        }

        private void OnHoverLeave(object? sender, EventArgs e)
        {
            if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
            {
                rowPanel.FillColor = normalFill;
                rowPanel.BorderColor = normalBorder;
                this.Cursor = Cursors.Default;
            }
        }

        public Action<EmailModel>? OnEmailClicked;

        private void OnRowClick(object? sender, MouseEventArgs e)
        {
            if (Email == null) return;

            if (OnDraftClicked != null)
            {
                OnDraftClicked.Invoke();
                return;
            }

            if (_gmailService == null) return;

            var home = Application.OpenForms.OfType<Home>().FirstOrDefault();
            if (home == null) return;

            // Mark as read on click — update UI immediately, then sync to Gmail
            if (!Email.IsRead)
            {
                Email.IsRead = true;
                guna2CirclePictureBox2.Visible = false;
                guna2HtmlLabel1.Font = new Font(guna2HtmlLabel1.Font, FontStyle.Regular);
                guna2HtmlLabel3.Location = new Point(guna2HtmlLabel3.Location.X + 20, guna2HtmlLabel3.Location.Y);
                _ = MarkAsReadAsync();
                home.RefreshBadges();
            }

            if (IsAnsweredRow)
            {
                home.ShowAnsweredContent(Email, _gmailService);
                return;
            }

            var compose = new Compose();
            compose.LoadEmail(Email, _gmailService);
            home.ShowFullscreenCompose(compose);
        }

        private async Task MarkAsReadAsync()
        {
            if (_gmailService == null || Email == null) return;
            try
            {
                var request = new Google.Apis.Gmail.v1.Data.ModifyMessageRequest
                {
                    RemoveLabelIds = new List<string> { "UNREAD" }
                };
                await _gmailService.Users.Messages.Modify(request, "me", Email.Id).ExecuteAsync();
                var match = AppState.Emails.FirstOrDefault(e => e.Id == Email.Id);
                if (match != null) match.IsRead = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mark as read failed: {ex.Message}");
            }
        }

        // ── Avatar helpers ────────────────────────────────────────────────────

        private void SetSenderAvatar(string fromName, string fromEmail)
        {
            try
            {
                string initials = GetInitials(fromName, fromEmail);
                Color avatarColor = GetAvatarColor(fromName + fromEmail);

                int size = guna2CirclePictureBox1.Width;
                var bmp = new Bitmap(size, size);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    g.FillEllipse(new SolidBrush(avatarColor), 0, 0, size, size);

                    float fontSize = size * 0.35f;
                    var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(initials, font, new SolidBrush(Color.White),
                        new RectangleF(0, 0, size, size), format);
                }

                guna2CirclePictureBox1.Image = bmp;
            }
            catch { /* leave default if anything fails */ }
        }

        private static string GetInitials(string name, string email)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
                if (parts.Length == 1 && parts[0].Length > 0)
                    return parts[0][0].ToString().ToUpper();
            }

            if (!string.IsNullOrWhiteSpace(email))
                return email[0].ToString().ToUpper();

            return "?";
        }

        private static Color GetAvatarColor(string seed)
        {
            Color[] palette =
            {
                Color.FromArgb(98,  117, 217),
                Color.FromArgb(76,  175, 130),
                Color.FromArgb(229, 115,  95),
                Color.FromArgb(100, 160, 220),
                Color.FromArgb(186, 104, 200),
                Color.FromArgb(77,  182, 172),
                Color.FromArgb(240, 154,  56),
                Color.FromArgb(129, 199, 132),
                Color.FromArgb(229, 115, 155),
                Color.FromArgb(111, 143, 175),
            };

            int hash = 0;
            foreach (char c in seed)
                hash = (hash * 31 + c) & 0x7fffffff;

            return palette[hash % palette.Length];
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
    }
}