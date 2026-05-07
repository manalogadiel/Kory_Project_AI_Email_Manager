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
            fullscreenCompose.TopLevel = false;
            fullscreenCompose.FormBorderStyle = FormBorderStyle.None;
            fullscreenCompose.Dock = DockStyle.Fill;
            this.Controls.Add(fullscreenCompose);

            // Hide all other controls (dock panels, etc.)
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl != fullscreenCompose)
                    ctrl.Visible = false;
            }

            fullscreenCompose.BringToFront();
            fullscreenCompose.Visible = true;
        }

        public void HideFullscreenCompose()
        {
            // Remove ANY Compose control that might be on the form
            var composeList = this.Controls.OfType<Compose>().ToList();
            foreach (var compose in composeList)
            {
                this.Controls.Remove(compose);
                compose.Dispose();
            }
            fullscreenCompose = null;

            // Show all other controls (they were hidden when Compose appeared)
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Visible = true;
            }

            // Force pnlMainContent to be visible and bring it to front
            pnlMainContent.Visible = true;
            pnlMainContent.BringToFront();

            // Bring Inbox (the categories) to the front inside pnlMainContent
            var inbox = pnlMainContent.Controls.OfType<Inbox>().FirstOrDefault();
            if (inbox != null)
            {
                inbox.BringToFront();
            }
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
