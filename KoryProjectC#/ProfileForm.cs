using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace KoryProjectC_
{
    public partial class ProfileForm : Form
    {
        public event EventHandler? OnSaved;

        public string FullName => txtFullName.Text;
        public string Title => txtTitle.Text;
        public string Department => txtDepartment.Text;
        public string ComplementaryClose => cboClose.Text;

        public ProfileForm()
        {
            InitializeComponent();
        }

        private void UpdatePreview()
        {
            string name = string.IsNullOrWhiteSpace(txtFullName.Text) ? "[Full Name]" : txtFullName.Text;
            string title = string.IsNullOrWhiteSpace(txtTitle.Text) ? "[Title]" : txtTitle.Text;
            string dept = string.IsNullOrWhiteSpace(txtDepartment.Text) ? "[Department]" : txtDepartment.Text;
            string close = cboClose.Text;

            txtPreview.Text = $"{close}\r\n\r\n{name}\r\n{title}\r\n{dept}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            OnSaved?.Invoke(this, EventArgs.Empty);
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TxtFullName_TextChanged(object sender, EventArgs e) => UpdatePreview();
        private void TxtTitle_TextChanged(object sender, EventArgs e) => UpdatePreview();
        private void TxtDepartment_TextChanged(object sender, EventArgs e) => UpdatePreview();
        private void CboClose_SelectedIndexChanged(object sender, EventArgs e) => UpdatePreview();
    }
}