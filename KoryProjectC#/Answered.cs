using Google.Apis.Gmail.v1;
using System;
using System.Linq;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class Answered : UserControl
    {
        public GmailService? _gmailService;

        public Answered()
        {
            InitializeComponent();
        }

        private void Answered_Load(object sender, EventArgs e)
        {
            var home = Application.OpenForms.OfType<Home>().FirstOrDefault();
            if (home?._gmailService != null)
            {
                _gmailService = home._gmailService;
                _ = LoadSentEmailsAsync();
            }
        }

        public async Task LoadSentEmailsAsync()
        {
            if (_gmailService == null) return;

            ECPanel.Controls.Clear();

            try
            {
                var sentEmails = await GmailHelper.FetchSentEmailsAsync(_gmailService);

                if (!sentEmails.Any()) return;

                foreach (var email in sentEmails)
                {
                    var row = new EmailRow();
                    row.SetEmail(email, _gmailService);
                    row.IsAnsweredRow = true;
                    row.Width = ECPanel.Width - 10;
                    ECPanel.Controls.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading sent emails",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }
    }
}