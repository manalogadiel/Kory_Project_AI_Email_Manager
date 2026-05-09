using Google.Apis.Gmail.v1;
using System.Drawing;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class EmailRow : UserControl
    {
        public EmailModel? Email { get; private set; }
        private GmailService? _gmailService;
        public bool IsAnsweredRow { get; set; } = false;
        private readonly Color normalFill = Color.FromArgb(26, 28, 46);
        private readonly Color hoverFill = Color.FromArgb(17, 18, 30);
        private readonly Color normalBorder = Color.FromArgb(39, 40, 64);
        private readonly Color hoverBorder = Color.FromArgb(43, 40, 89);

        public EmailRow()
        {
            InitializeComponent();
            AttachEvents(this);
        }

        public void SetEmail(EmailModel email, GmailService service)
        {
            Email = email;
            _gmailService = service;
            guna2HtmlLabel1.Text = email.FromName;
            guna2HtmlLabel2.Text = email.Snippet;
            guna2HtmlLabel3.Text = email.Date;

            if (!email.IsRead)
                guna2HtmlLabel1.Font = new Font(
                    guna2HtmlLabel1.Font, FontStyle.Bold);
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
            if (Email == null || _gmailService == null) return;

            var home = Application.OpenForms.OfType<Home>().FirstOrDefault();
            if (home == null) return;

            if (IsAnsweredRow)
            {
                home.ShowAnsweredContent(Email, _gmailService);
                return;
            }

            var compose = new Compose();
            compose.LoadEmail(Email, _gmailService);
            home.ShowFullscreenCompose(compose);
        }
    }
}