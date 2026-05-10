using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace KoryProjectC_
{
    public partial class Inbox : UserControl
    {
        private Dictionary<string, int> originalPositions = new();
        private Control? activeControl;
        private int currentTargetY;

        private record CategoryMap(
            string Name,
            Guna2HtmlLabel CategoryLabel,
            Guna2HtmlLabel SubjectLabel,
            Guna2HtmlLabel CountLabel,
            Guna2Button DateLabel,
            Guna2Button Badge,
            Guna2Panel Panel);

        private List<CategoryMap> _maps = new();

        public Inbox()
        {
            InitializeComponent();
            animationTimer.Interval = 10;
            animationTimer.Enabled = false;

            BuildCategoryMaps();
            LoadCategoryData();

            RefreshBtn.Click += RefreshBtn_Click;
        }

        private async void RefreshBtn_Click(object? sender, EventArgs e)
        {
            var home = Application.OpenForms.OfType<Home>().FirstOrDefault();
            if (home == null) return;

            RefreshBtn.Enabled = false;

            try
            {
                await home.RefreshAllAsync();
            }
            finally
            {
                RefreshBtn.Enabled = true;
            }
        }

        private void BuildCategoryMaps()
        {
            _maps = new List<CategoryMap>
            {
                new("GRADE CONCERNS",    guna2HtmlLabel1,  guna2HtmlLabel2,  guna2HtmlLabel3,  guna2Button2,  guna2Button1,  catGrade),
                new("ABSENTS / EXCUSES", guna2HtmlLabel6,  guna2HtmlLabel5,  guna2HtmlLabel4,  guna2Button4,  guna2Button3,  catAbsent),
                new("REQUESTS",          guna2HtmlLabel9,  guna2HtmlLabel8,  guna2HtmlLabel7,  guna2Button6,  guna2Button5,  catRequest),
                new("ACADEMIC CONCERNS", guna2HtmlLabel18, guna2HtmlLabel17, guna2HtmlLabel16, guna2Button12, guna2Button11, catConcern),
                new("REQUIREMENTS",      guna2HtmlLabel15, guna2HtmlLabel14, guna2HtmlLabel13, guna2Button10, guna2Button9,  catRequirement),
                new("NON-ACADEMIC",      guna2HtmlLabel12, guna2HtmlLabel11, guna2HtmlLabel10, guna2Button8,  guna2Button7,  catNon),
            };
        }

        public void LoadCategoryData()
        {
            catGrade.MouseClick -= category_Click;
            catAbsent.MouseClick -= category_Click;
            catRequest.MouseClick -= category_Click;

            foreach (var map in _maps)
            {
                var emails = AppState.Emails
                    .Where(e => e.Category == map.Name)
                    .ToList();

                int total = emails.Count;
                int unread = emails.Count(e => !e.IsRead);
                var latest = emails.FirstOrDefault();

                map.CategoryLabel.Text = map.Name;
                map.SubjectLabel.Text = latest?.Subject ?? "No emails yet";
                map.CountLabel.Text = $"{total} email{(total != 1 ? "s" : "")}";
                map.DateLabel.Text = latest?.Date ?? "";

                map.Badge.Enabled = true;
                map.Badge.FillColor = Color.FromArgb(107, 92, 231);
                map.Badge.Text = $"{unread} new";

                var cat = map.Name;
                map.Panel.MouseClick += (_, _) => OpenCategory(cat);
            }
        }

        public void RefreshCategoryData() => LoadCategoryData();

        private void OpenCategory(string category)
        {
            if (this.Parent == null) return;

            var emails = AppState.Emails
                .Where(e => e.Category == category)
                .ToList();

            Control parent = this.Parent;

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is EmailContent existing)
                {
                    existing.LoadEmails(emails, category, AppState.GmailService!);
                    existing.BringToFront();
                    return;
                }
            }

            var view = new EmailContent();
            view.Dock = DockStyle.Fill;
            parent.Controls.Add(view);
            view.LoadEmails(emails, category, AppState.GmailService!);
            view.BringToFront();
        }

        // ── Animation ─────────────────────────────────────────────────────────

        private void ResetAllOtherPanels(Control current)
        {
            foreach (Control ctrl in guna2Panel1.Controls)
            {
                if (ctrl is Guna2Panel && ctrl != current)
                    if (originalPositions.TryGetValue(ctrl.Name, out int y))
                        ctrl.Location = new Point(ctrl.Location.X, y);
            }
        }

        private void category_MouseEnter(object sender, EventArgs e)
        {
            activeControl = sender as Control;
            if (activeControl == null) return;

            if (!originalPositions.ContainsKey(activeControl.Name))
                originalPositions[activeControl.Name] = activeControl.Location.Y;

            ResetAllOtherPanels(activeControl);
            currentTargetY = originalPositions[activeControl.Name] - 10;
            animationTimer.Start();
        }

        private void category_MouseLeave(object sender, EventArgs e)
        {
            if (sender is not Control ctrl) return;
            if (!ctrl.ClientRectangle.Contains(ctrl.PointToClient(Cursor.Position)))
            {
                activeControl = ctrl;
                if (originalPositions.TryGetValue(activeControl.Name, out int orig))
                {
                    currentTargetY = orig;
                    animationTimer.Start();
                }
            }
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            if (activeControl == null) return;
            int y = activeControl.Location.Y;
            const int speed = 2;

            if (y > currentTargetY)
                activeControl.Location = new Point(activeControl.Location.X,
                    Math.Max(currentTargetY, y - speed));
            else if (y < currentTargetY)
                activeControl.Location = new Point(activeControl.Location.X,
                    Math.Min(currentTargetY, y + speed));
            else
                animationTimer.Stop();
        }

        // Designer stubs
        private void category_Click(object sender, EventArgs e) { }
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
        private void category1_Paint(object sender, PaintEventArgs e) { }
        private void guna2Button1_Click(object sender, EventArgs e) { }
        private void guna2Button2_Click(object sender, EventArgs e) { }
        private void guna2ImageButton1_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void guna2Button10_Click(object sender, EventArgs e) { }
    }
}