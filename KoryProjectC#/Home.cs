using Google.Apis.Gmail.v1;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class Home : Form
    {
        private Inbox ucInbox = null!;
        private InProgress ucInProgress = null!;
        private Answered ucAnswered = null!;
        private Compose? fullscreenCompose;
        public GmailService? _gmailService;

        public Home()
        {
            InitializeComponent();
            SetupDashboard();
        }

        private async void Home_Load(object sender, EventArgs e)
        {
            UpdateHeaderStats(pending: 0, answered: 0, avgRespMinutes: 0);

            try
            {
                _gmailService = await GmailHelper.AuthenticateAsync();
                await RefreshStatsAsync();
            }
            catch (Exception ex)
            {
                placehold.Text = "Could not connect to Gmail.";
                MessageBox.Show(ex.Message, "Gmail Auth Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task RefreshStatsAsync()
        {
            if (_gmailService == null) return;

            var emailsTask = GmailHelper.FetchEmailsAsync(_gmailService);
            var answeredTask = GmailHelper.GetSentTodayCountAsync(_gmailService);
            var avgRespTask = GmailHelper.GetAvgResponseMinutesAsync(_gmailService);
            var nameTask = GmailHelper.GetUserNameAsync(_gmailService);
            var picTask = GmailHelper.GetProfilePictureAsync();

            await Task.WhenAll(emailsTask, answeredTask, avgRespTask, nameTask, picTask);

            var emails = await emailsTask;
            int pending = emails.Count(e => !e.IsRead);
            int answered = await answeredTask;
            int avgResp = (int)Math.Round(await avgRespTask);
            string name = await nameTask;

            AppState.UserName = name;
            AppState.UserEmail = (await _gmailService.Users.GetProfile("me").ExecuteAsync()).EmailAddress ?? "";

            var pic = await picTask;
            if (pic != null)
            {
                var resized = new Bitmap(profilePicture.Width, profilePicture.Height);
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawImage(pic, 0, 0, profilePicture.Width, profilePicture.Height);
                }
                profilePicture.Image = resized;
            }

            title.Text = $"Hi, {name}!";
            UpdateHeaderStats(pending, answered, avgResp);

            ucAnswered._gmailService = _gmailService;
            await ucAnswered.LoadSentEmailsAsync();
        }

        public void UpdateHeaderStats(int pending, int answered, int avgRespMinutes)
        {
            placehold.Text =
                $"You have {pending} unread email{(pending == 1 ? "" : "s")} " +
                "waiting.  Let's tackle them!";

            npPlaceholder.Text = pending.ToString();
            atPlaceholder.Text = answered.ToString();
            arPlaceholder.Text = avgRespMinutes > 0 ? $"{avgRespMinutes}m" : "—";
        }

        // ── COMPOSE ───────────────────────────────────────────────────────────

        public void ShowFullscreenCompose(Compose compose)
        {
            fullscreenCompose = compose;
            compose.Size = this.ClientSize;
            compose.Location = new Point(0, 0);
            this.Controls.Add(compose);
            compose.BringToFront();
            compose.Visible = true;
        }

        public void HideFullscreenCompose()
        {
            foreach (var ctrl in this.Controls.OfType<Compose>().ToList())
            {
                this.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
            fullscreenCompose = null;

            if (ucInProgress.IsHandleCreated)
                ucInProgress.LoadDrafts();

            var emailContent = pnlMainContent.Controls.OfType<EmailContent>().FirstOrDefault();
            if (emailContent != null)
            {
                emailContent.BringToFront();
                return;
            }

            ucInbox.BringToFront();
        }

        // ── ANSWERED CONTENT ──────────────────────────────────────────────────

        public void ShowAnsweredContent(EmailModel sentEmail, GmailService service)
        {
            // Remove any existing AnsweredContent
            foreach (var ctrl in this.Controls.OfType<AnsweredContent>().ToList())
            {
                this.Controls.Remove(ctrl);
                ctrl.Dispose();
            }

            var content = new AnsweredContent();
            content.TopLevel = false;
            content.FormBorderStyle = FormBorderStyle.None;
            content.Size = this.ClientSize;
            content.Location = new Point(0, 0);

            this.Controls.Add(content);
            content.BringToFront();
            content.Visible = true;

            _ = content.LoadAsync(sentEmail, service);
        }

        public void ShowAnswered()
        {
            foreach (var ctrl in this.Controls.OfType<AnsweredContent>().ToList())
            {
                this.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
            ucAnswered.BringToFront();
        }

        // ── DASHBOARD SETUP ───────────────────────────────────────────────────

        private void SetupDashboard()
        {
            ucInbox = new Inbox();
            ucInProgress = new InProgress();
            ucAnswered = new Answered();

            ucInbox.Dock = DockStyle.Fill;
            ucInProgress.Dock = DockStyle.Fill;
            ucAnswered.Dock = DockStyle.Fill;

            pnlMainContent.Controls.Add(ucInbox);
            pnlMainContent.Controls.Add(ucInProgress);
            pnlMainContent.Controls.Add(ucAnswered);

            ucInbox.BringToFront();
        }

        // ── BUTTONS ───────────────────────────────────────────────────────────

        private void composeBtn_Click(object sender, EventArgs e)
        {
            SingleCompose compose = new SingleCompose(_gmailService);
            compose.ShowDialog(this);
        }

        private void InboxBtn_Click(object sender, EventArgs e) => ucInbox.BringToFront();

        private void InProgressBtn_Click(object sender, EventArgs e)
        {
            ucInProgress.LoadDrafts();
            ucInProgress.BringToFront();
        }

        private void AnsweredBtn_Click(object sender, EventArgs e) => ucAnswered.BringToFront();

        private void logOutBtn_Click(object sender, EventArgs e)
        {
            var confirm = new logOutConfirmation();
            confirm.StartPosition = FormStartPosition.CenterParent;
            confirm.ShowDialog(this);

            if (!confirm.Confirmed) return;

            string tokenPath = Path.Combine(Application.StartupPath, "token_store");
            if (Directory.Exists(tokenPath))
                foreach (var file in Directory.GetFiles(tokenPath))
                    File.Delete(file);

            var login = new LoginForm();
            login.Show();
            this.Close();
        }

        public async Task RefreshAfterSendAsync() => await RefreshStatsAsync();

        private void pnlMainContent_Paint(object sender, PaintEventArgs e) { }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void placehold_Click(object sender, EventArgs e) { }
        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e) { }
        private void guna2Panel7_Paint(object sender, PaintEventArgs e) { }
        private void guna2Panel6_Paint(object sender, PaintEventArgs e) { }
        private void guna2Panel8_Paint(object sender, PaintEventArgs e) { }
        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e) { }
        private void guna2HtmlLabel1_Click_2(object sender, EventArgs e) { }
        private void guna2Button1_Click(object sender, EventArgs e) { }
    }
}