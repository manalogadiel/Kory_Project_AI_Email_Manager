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
            logOutNo = new Guna.UI2.WinForms.Guna2Button();
            logOutYES = new Guna.UI2.WinForms.Guna2Button();
            logOutTextConfirmation = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            SuspendLayout();
            // 
            // logOutNo
            // 
            logOutNo.BackColor = Color.FromArgb(66, 81, 162);
            logOutNo.BorderColor = Color.White;
            logOutNo.BorderRadius = 4;
            logOutNo.BorderThickness = 2;
            logOutNo.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            logOutNo.CheckedState.FillColor = Color.FromArgb(28, 27, 51);
            logOutNo.CheckedState.ForeColor = Color.White;
            logOutNo.CustomizableEdges = customizableEdges1;
            logOutNo.DisabledState.BorderColor = Color.DarkGray;
            logOutNo.DisabledState.CustomBorderColor = Color.DarkGray;
            logOutNo.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            logOutNo.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            logOutNo.FillColor = Color.Transparent;
            logOutNo.Font = new Font("League Spartan", 12F);
            logOutNo.ForeColor = Color.FromArgb(142, 144, 166);
            logOutNo.HoverState.FillColor = Color.FromArgb(23, 24, 29);
            logOutNo.HoverState.ForeColor = Color.White;
            logOutNo.ImageAlign = HorizontalAlignment.Left;
            logOutNo.Location = new Point(25, 145);
            logOutNo.Margin = new Padding(0);
            logOutNo.Name = "logOutNo";
            logOutNo.RightToLeft = RightToLeft.No;
            logOutNo.ShadowDecoration.CustomizableEdges = customizableEdges2;
            logOutNo.Size = new Size(143, 48);
            logOutNo.TabIndex = 4;
            logOutNo.Text = "NO";
            logOutNo.Click += logOutNo_Click;
            // 
            // logOutYES
            // 
            logOutYES.BackColor = Color.Transparent;
            logOutYES.BorderColor = Color.White;
            logOutYES.BorderRadius = 4;
            logOutYES.BorderThickness = 2;
            logOutYES.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            logOutYES.CheckedState.FillColor = Color.FromArgb(28, 27, 51);
            logOutYES.CheckedState.ForeColor = Color.White;
            logOutYES.CustomizableEdges = customizableEdges3;
            logOutYES.DisabledState.BorderColor = Color.DarkGray;
            logOutYES.DisabledState.CustomBorderColor = Color.DarkGray;
            logOutYES.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            logOutYES.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            logOutYES.FillColor = Color.Transparent;
            logOutYES.Font = new Font("League Spartan", 12F);
            logOutYES.ForeColor = Color.FromArgb(142, 144, 166);
            logOutYES.HoverState.FillColor = Color.FromArgb(23, 24, 29);
            logOutYES.HoverState.ForeColor = Color.White;
            logOutYES.ImageAlign = HorizontalAlignment.Left;
            logOutYES.Location = new Point(223, 145);
            logOutYES.Margin = new Padding(0);
            logOutYES.Name = "logOutYES";
            logOutYES.RightToLeft = RightToLeft.No;
            logOutYES.ShadowDecoration.CustomizableEdges = customizableEdges4;
            logOutYES.Size = new Size(143, 48);
            logOutYES.TabIndex = 4;
            logOutYES.Text = "YES";
            logOutYES.UseTransparentBackground = true;
            logOutYES.Click += logOutYES_Click;
            // 
            // logOutTextConfirmation
            // 
            logOutTextConfirmation.BackColor = Color.Transparent;
            logOutTextConfirmation.Font = new Font("League Spartan SemiBold", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            logOutTextConfirmation.ForeColor = Color.White;
            logOutTextConfirmation.Location = new Point(25, 45);
            logOutTextConfirmation.Margin = new Padding(3, 4, 3, 4);
            logOutTextConfirmation.Name = "logOutTextConfirmation";
            logOutTextConfirmation.Size = new Size(325, 39);
            logOutTextConfirmation.TabIndex = 2;
            logOutTextConfirmation.Text = "Are you sure want to Log Out?";
            logOutTextConfirmation.Click += title_Click;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("League Spartan Medium", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.Location = new Point(46, 49);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(303, 47);
            guna2HtmlLabel1.TabIndex = 5;
            guna2HtmlLabel1.Text = "Are you sure to log out?";
            // 
            // logOutConfirmation
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(66, 81, 162);
            ClientSize = new Size(397, 233);
            Controls.Add(guna2HtmlLabel1);
            Controls.Add(logOutYES);
            Controls.Add(logOutNo);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "logOutConfirmation";
            Text = "logOutConfirmation";
            TransparencyKey = Color.Transparent;
            Load += logOutConfirmation_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Guna.UI2.WinForms.Guna2Button logOutNo;
        private Guna.UI2.WinForms.Guna2Button logOutYES;
        private Guna.UI2.WinForms.Guna2HtmlLabel logOutTextConfirmation;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
    }
}