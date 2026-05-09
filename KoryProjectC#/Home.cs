using Google.Apis.Gmail.v1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        private GmailService? _gmailService;

        public Home()
        {
            InitializeComponent();
            SetupDashboard();
        }

        private async void Home_Load(object sender, EventArgs e)
        {
            // Show zeroes while Gmail loads
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

        // ════════════════════════════════════════════════════════════════════════
        //  FETCH + UPDATE STATS
        // ════════════════════════════════════════════════════════════════════════

        private async Task RefreshStatsAsync()
        {
            if (_gmailService == null) return;

            // Run all three calls at the same time
            var emailsTask = GmailHelper.FetchEmailsAsync(_gmailService);
            var answeredTask = GmailHelper.GetSentTodayCountAsync(_gmailService);
            var avgRespTask = GmailHelper.GetAvgResponseMinutesAsync(_gmailService);

            await Task.WhenAll(emailsTask, answeredTask, avgRespTask);

            var emails = await emailsTask;
            int pending = emails.Count(e => !e.IsRead);
            int answered = await answeredTask;
            int avgResp = (int)Math.Round(await avgRespTask);

            UpdateHeaderStats(pending, answered, avgResp);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  STAT CARD UPDATE  —  call this anytime to push new numbers in
        // ════════════════════════════════════════════════════════════════════════

        public void UpdateHeaderStats(int pending, int answered, int avgRespMinutes)
        {
            // Subtitle
            placehold.Text =
                $"You have {pending} unread email{(pending == 1 ? "" : "s")} " +
                "waiting.  Let's tackle them!";

            // TODO: replace label names below with the ones you set in the designer

            // New Pending card
            npPlaceholder.Text = pending.ToString();

            // Answered Today card
            atPlaceholder.Text = answered.ToString();

            // Avg. Response card
            arPlaceholder.Text = avgRespMinutes > 0 ? $"{avgRespMinutes}m" : "—";
        }

        // ════════════════════════════════════════════════════════════════════════
        //  EXISTING METHODS (unchanged)
        // ════════════════════════════════════════════════════════════════════════

        public void ShowFullscreenCompose(Compose compose)
        {
            fullscreenCompose = compose;
            this.Controls.Add(compose);
            compose.Size = this.ClientSize;
            compose.Location = new Point(0, 0);
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

            var emailContent = pnlMainContent.Controls.OfType<EmailContent>().FirstOrDefault();
            if (emailContent != null)
            {
                emailContent.BringToFront();
                return;
            }

            ucInbox.BringToFront();
        }

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

        private void composeBtn_Click(object sender, EventArgs e)
        {
            SingleCompose compose = new SingleCompose();
            compose.ShowDialog(this); // 'this' sets Home as the owner
        }

        private void InboxBtn_Click(object sender, EventArgs e) => ucInbox.BringToFront();
        private void InProgressBtn_Click(object sender, EventArgs e) => ucInProgress.BringToFront();
        private void AnsweredBtn_Click(object sender, EventArgs e) => ucAnswered.BringToFront();

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
    }
}