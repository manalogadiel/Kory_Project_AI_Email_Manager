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
        private static readonly string ProfilePath = Path.Combine(
            Application.StartupPath, "profile.json");

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

        public void LoadSavedProfile()
        {
            if (!File.Exists(ProfilePath)) return;
            try
            {
                var json = File.ReadAllText(ProfilePath);
                var data = System.Text.Json.JsonSerializer.Deserialize<ProfileData>(json);
                if (data == null) return;

                txtFullName.Text = data.FullName ?? "";
                txtTitle.Text = data.Title ?? "";
                txtDepartment.Text = data.Department ?? "";

                int idx = cboClose.Items.IndexOf(data.ComplementaryClose ?? "");
                if (idx >= 0) cboClose.SelectedIndex = idx;

                UpdatePreview();
            }
            catch { }
        }

        private void SaveProfile()
        {
            var data = new ProfileData
            {
                FullName = txtFullName.Text,
                Title = txtTitle.Text,
                Department = txtDepartment.Text,
                ComplementaryClose = cboClose.Text
            };
            File.WriteAllText(ProfilePath,
                System.Text.Json.JsonSerializer.Serialize(data));
        }

        internal class ProfileData
        {
            public string? FullName { get; set; }
            public string? Title { get; set; }
            public string? Department { get; set; }
            public string? ComplementaryClose { get; set; }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveProfile();
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