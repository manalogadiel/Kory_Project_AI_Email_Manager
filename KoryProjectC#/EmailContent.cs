using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class EmailContent : UserControl
    {
        public EmailContent()
        {
            InitializeComponent();

            ECPanel.AutoScroll = false;
            ECPanel.HorizontalScroll.Maximum = 0;
            ECPanel.HorizontalScroll.Enabled = false;
            ECPanel.HorizontalScroll.Visible = false;
            ECPanel.AutoScroll = true;
        }

        /// <summary>Load real emails for the selected category.</summary>
        public void LoadEmails(List<EmailModel> emails, string category)
        {
            guna2HtmlLabel1.Text = category;
            guna2HtmlLabel2.Text =
                $"{emails.Count} email{(emails.Count != 1 ? "s" : "")} in this category";

            ECPanel.Controls.Clear();

            foreach (var email in emails)
            {
                var card = new EmailRow();
                card.Width = ECPanel.Width - 25;
                card.SetEmail(email);
                ECPanel.Controls.Add(card);
            }
        }

        /// <summary>Legacy method kept for compatibility.</summary>
        public void AddCards(int count = 3)
        {
            for (int i = 0; i < count; i++)
            {
                var card = new EmailRow();
                card.Width = ECPanel.Width - 25;
                ECPanel.Controls.Add(card);
            }
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            if (this.Parent == null) return;

            foreach (Control ctrl in this.Parent.Controls)
            {
                if (ctrl is Inbox inbox)
                {
                    inbox.BringToFront();
                    return;
                }
            }
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
    }
}