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

        public Home()
        {
            InitializeComponent();
            SetupDashboard();
        }

        public void ShowFullscreenCompose(Compose compose)
        {
            fullscreenCompose = compose;

            // Add to the FORM itself, not pnlMainContent
            this.Controls.Add(compose);
            compose.Size = this.ClientSize;   // cover the entire window
            compose.Location = new Point(0, 0);
            compose.BringToFront();
            compose.Visible = true;
        }

        public void HideFullscreenCompose()
        {
            // Remove from the FORM's controls, not pnlMainContent
            foreach (var ctrl in this.Controls.OfType<Compose>().ToList())
            {
                this.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
            fullscreenCompose = null;

            // Return to EmailContent if open, otherwise Inbox
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

        private void Home_Load(object sender, EventArgs e)
        {
            // your load logic here (can leave empty)  
        }

        private void InboxBtn_Click(object sender, EventArgs e)
        {
            ucInbox.BringToFront();
        }

        private void InProgressBtn_Click(object sender, EventArgs e)
        {
            ucInProgress.BringToFront();
        }

        private void AnsweredBtn_Click(object sender, EventArgs e)
        {
            ucAnswered.BringToFront();
        }

        private void pnlMainContent_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
