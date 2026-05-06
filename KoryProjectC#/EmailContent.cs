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

    }
}
