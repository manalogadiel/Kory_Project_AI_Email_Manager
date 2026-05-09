using System;
using System.Drawing;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public class EmailTypeDialog : Form
    {
        public string SelectedType { get; private set; } = "";
        public string ExtraContext { get; private set; } = "";

        private ComboBox cmbType = new ComboBox();
        private TextBox txtContext = new TextBox();
        private Button btnOk = new Button();
        private Button btnCancel = new Button();

        public EmailTypeDialog()
        {
            this.Text = "What type of email?";
            this.Size = new Size(400, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblType = new Label
            {
                Text = "Email Type:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            cmbType.Location = new Point(20, 42);
            cmbType.Width = 340;
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Items.AddRange(new[]
            {
                "Grade Inquiry",
                "Absence / Excuse Letter",
                "Request Letter",
                "Academic Concern",
                "Announcement",
                "Reminder",
                "Follow-up",
                "General"
            });
            cmbType.SelectedIndex = 0;

            var lblContext = new Label
            {
                Text = "Additional context (optional):",
                Location = new Point(20, 85),
                AutoSize = true
            };

            txtContext.Location = new Point(20, 107);
            txtContext.Size = new Size(340, 70);
            txtContext.Multiline = true;
            txtContext.ScrollBars = ScrollBars.Vertical;
            txtContext.PlaceholderText = "e.g. Student is requesting grade reconsideration for Midterms...";

            btnOk.Text = "Generate";
            btnOk.Location = new Point(200, 190);
            btnOk.Size = new Size(80, 30);
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Click += (s, e) =>
            {
                SelectedType = cmbType.SelectedItem?.ToString() ?? "General";
                ExtraContext = txtContext.Text.Trim();
                this.Close();
            };

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(290, 190);
            btnCancel.Size = new Size(70, 30);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblType, cmbType, lblContext, txtContext, btnOk, btnCancel
            });

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}