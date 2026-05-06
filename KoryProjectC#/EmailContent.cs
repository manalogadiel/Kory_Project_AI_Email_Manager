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
        private void BackBtn_Click(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                // 1. Create the Inbox screen
                Inbox mainInbox = new Inbox();
                mainInbox.Dock = DockStyle.Fill;

                // 2. Clear this EmailContent from the parent and add the Inbox back
                Control parentContainer = this.Parent;
                parentContainer.Controls.Clear();
                parentContainer.Controls.Add(mainInbox);
            }
        }
        public void AddCards(int count = 3)
        {
            // Clear existing cards if necessary
            // flowLayoutPanel1.Controls.Clear();

            for (int i = 0; i < count; i++)
            {
                // 1. Create an instance of your card
                EmailRow card = new EmailRow();

                // 2. Set the width to match the container (minus scrollbar width)
                card.Width = ECPanel.Width - 25;

                // 3. Optional: Pass data to the card
                // card.Title = "Card Number " + i;

                // 4. Add it to the FlowLayoutPanel
                ECPanel.Controls.Add(card);
            }
        }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
