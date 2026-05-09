using System;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class SingleCompose : Form
    {
        public SingleCompose()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void SingleCompose_Load(object sender, EventArgs e)
        {
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetRecipient() => toTextBox.Text.Trim();
        private string GetSubject() => subjectTextBox.Text.Trim();

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {

        }

        private void SendBtn_Click(object sender, EventArgs e)
        {

        }
    }
}