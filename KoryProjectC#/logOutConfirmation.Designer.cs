namespace KoryProjectC_
{
    partial class logOutConfirmation
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            logOutTextConfirmation = new Guna.UI2.WinForms.Guna2HtmlLabel();
            logOutYES = new Guna.UI2.WinForms.Guna2Button();
            logOutNo = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // logOutTextConfirmation
            // 
            logOutTextConfirmation.BackColor = Color.Transparent;
            logOutTextConfirmation.Font = new Font("League Spartan", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            logOutTextConfirmation.ForeColor = Color.White;
            logOutTextConfirmation.Location = new Point(27, 45);
            logOutTextConfirmation.Margin = new Padding(3, 4, 3, 4);
            logOutTextConfirmation.Name = "logOutTextConfirmation";
            logOutTextConfirmation.Size = new Size(341, 39);
            logOutTextConfirmation.TabIndex = 2;
            logOutTextConfirmation.Text = "Are you sure want to Log Out?";
            logOutTextConfirmation.Click += title_Click;
            // 
            // logOutYES
            // 
            logOutYES.BorderColor = Color.Silver;
            logOutYES.BorderRadius = 20;
            logOutYES.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            logOutYES.CheckedState.FillColor = Color.FromArgb(28, 27, 51);
            logOutYES.CheckedState.ForeColor = Color.White;
            logOutYES.CustomizableEdges = customizableEdges5;
            logOutYES.DisabledState.BorderColor = Color.DarkGray;
            logOutYES.DisabledState.CustomBorderColor = Color.DarkGray;
            logOutYES.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            logOutYES.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            logOutYES.FillColor = Color.Transparent;
            logOutYES.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            logOutYES.ForeColor = Color.FromArgb(142, 144, 166);
            logOutYES.HoverState.FillColor = Color.FromArgb(23, 24, 29);
            logOutYES.HoverState.ForeColor = Color.White;
            logOutYES.ImageAlign = HorizontalAlignment.Left;
            logOutYES.Location = new Point(225, 135);
            logOutYES.Name = "logOutYES";
            logOutYES.ShadowDecoration.CustomizableEdges = customizableEdges6;
            logOutYES.Size = new Size(143, 48);
            logOutYES.TabIndex = 3;
            logOutYES.Text = "YES";
            logOutYES.Click += logOutYES_Click;
            // 
            // logOutNo
            // 
            logOutNo.BorderColor = Color.Silver;
            logOutNo.BorderRadius = 20;
            logOutNo.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            logOutNo.CheckedState.FillColor = Color.FromArgb(28, 27, 51);
            logOutNo.CheckedState.ForeColor = Color.White;
            logOutNo.CustomizableEdges = customizableEdges7;
            logOutNo.DisabledState.BorderColor = Color.DarkGray;
            logOutNo.DisabledState.CustomBorderColor = Color.DarkGray;
            logOutNo.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            logOutNo.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            logOutNo.FillColor = Color.Transparent;
            logOutNo.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            logOutNo.ForeColor = Color.FromArgb(142, 144, 166);
            logOutNo.HoverState.FillColor = Color.FromArgb(23, 24, 29);
            logOutNo.HoverState.ForeColor = Color.White;
            logOutNo.ImageAlign = HorizontalAlignment.Left;
            logOutNo.Location = new Point(27, 135);
            logOutNo.Name = "logOutNo";
            logOutNo.ShadowDecoration.CustomizableEdges = customizableEdges8;
            logOutNo.Size = new Size(143, 48);
            logOutNo.TabIndex = 4;
            logOutNo.Text = "NO";
            logOutNo.Click += logOutNo_Click;
            // 
            // logOutConfirmation
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(66, 81, 162);
            ClientSize = new Size(397, 233);
            Controls.Add(logOutNo);
            Controls.Add(logOutYES);
            Controls.Add(logOutTextConfirmation);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "logOutConfirmation";
            Text = "logOutConfirmation";
            Load += logOutConfirmation_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel logOutTextConfirmation;
        private Guna.UI2.WinForms.Guna2Button logOutYES;
        private Guna.UI2.WinForms.Guna2Button logOutNo;
    }
}