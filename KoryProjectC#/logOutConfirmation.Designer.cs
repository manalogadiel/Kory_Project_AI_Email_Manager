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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            logOutNo = new Guna.UI2.WinForms.Guna2Button();
            logOutYES = new Guna.UI2.WinForms.Guna2Button();
            logOutTextConfirmation = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            guna2CustomGradientPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // logOutNo
            // 
            logOutNo.BackColor = Color.Transparent;
            logOutNo.BorderColor = Color.SkyBlue;
            logOutNo.BorderRadius = 20;
            logOutNo.BorderThickness = 1;
            logOutNo.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            logOutNo.CheckedState.FillColor = Color.FromArgb(28, 27, 51);
            logOutNo.CheckedState.ForeColor = Color.White;
            logOutNo.CustomizableEdges = customizableEdges1;
            logOutNo.FillColor = Color.FromArgb(38, 37, 81);
            logOutNo.Font = new Font("League Spartan", 12F);
            logOutNo.ForeColor = Color.DarkGray;
            logOutNo.HoverState.FillColor = Color.FromArgb(23, 24, 29);
            logOutNo.HoverState.ForeColor = Color.White;
            logOutNo.Location = new Point(60, 115);
            logOutNo.Name = "logOutNo";
            logOutNo.ShadowDecoration.CustomizableEdges = customizableEdges2;
            logOutNo.Size = new Size(183, 72);
            logOutNo.TabIndex = 4;
            logOutNo.Text = "NO";
            logOutNo.Click += logOutNo_Click;
            // 
            // logOutYES
            // 
            logOutYES.BackColor = Color.Transparent;
            logOutYES.BorderColor = Color.SkyBlue;
            logOutYES.BorderRadius = 20;
            logOutYES.BorderThickness = 1;
            logOutYES.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            logOutYES.CheckedState.FillColor = Color.FromArgb(28, 27, 51);
            logOutYES.CheckedState.ForeColor = Color.White;
            logOutYES.CustomizableEdges = customizableEdges3;
            logOutYES.FillColor = Color.FromArgb(38, 37, 81);
            logOutYES.Font = new Font("League Spartan", 12F);
            logOutYES.ForeColor = Color.DarkGray;
            logOutYES.HoverState.FillColor = Color.FromArgb(23, 24, 29);
            logOutYES.HoverState.ForeColor = Color.White;
            logOutYES.Location = new Point(279, 115);
            logOutYES.Name = "logOutYES";
            logOutYES.ShadowDecoration.CustomizableEdges = customizableEdges4;
            logOutYES.Size = new Size(183, 72);
            logOutYES.TabIndex = 4;
            logOutYES.Text = "YES";
            logOutYES.UseTransparentBackground = true;
            logOutYES.Click += logOutYES_Click;
            // 
            // logOutTextConfirmation
            // 
            logOutTextConfirmation.BackColor = Color.Transparent;
            logOutTextConfirmation.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            logOutTextConfirmation.ForeColor = Color.White;
            logOutTextConfirmation.Location = new Point(95, 37);
            logOutTextConfirmation.Name = "logOutTextConfirmation";
            logOutTextConfirmation.Size = new Size(355, 38);
            logOutTextConfirmation.TabIndex = 5;
            logOutTextConfirmation.Text = "DO YOU WANT TO LOG OUT?";
            logOutTextConfirmation.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.Controls.Add(logOutTextConfirmation);
            guna2CustomGradientPanel1.Controls.Add(logOutNo);
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges5;
            guna2CustomGradientPanel1.FillColor = Color.FromArgb(22, 23, 50);
            guna2CustomGradientPanel1.FillColor2 = Color.FromArgb(77, 75, 115);
            guna2CustomGradientPanel1.FillColor3 = Color.FromArgb(18, 27, 51);
            guna2CustomGradientPanel1.FillColor4 = Color.FromArgb(12, 23, 50);
            guna2CustomGradientPanel1.Location = new Point(0, 0);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2CustomGradientPanel1.Size = new Size(536, 224);
            guna2CustomGradientPanel1.TabIndex = 6;
            // 
            // logOutConfirmation
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(22, 23, 50);
            ClientSize = new Size(536, 224);
            Controls.Add(logOutYES);
            Controls.Add(guna2CustomGradientPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "logOutConfirmation";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "logOutConfirmation";
            Load += logOutConfirmation_Load;
            guna2CustomGradientPanel1.ResumeLayout(false);
            guna2CustomGradientPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Guna.UI2.WinForms.Guna2Button logOutNo;
        private Guna.UI2.WinForms.Guna2Button logOutYES;
        private Guna.UI2.WinForms.Guna2HtmlLabel logOutTextConfirmation;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
    }
}