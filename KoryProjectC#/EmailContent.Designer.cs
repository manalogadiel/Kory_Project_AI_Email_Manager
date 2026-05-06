namespace KoryProjectC_
{
    partial class EmailContent
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            BackBtn = new Guna.UI2.WinForms.Guna2Button();
            rowPanel = new Guna.UI2.WinForms.Guna2Panel();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ECPanel = new FlowLayoutPanel();
            guna2vScrollBar1 = new Guna.UI2.WinForms.Guna2VScrollBar();
            rowPanel.SuspendLayout();
            SuspendLayout();
            // 
            // BackBtn
            // 
            BackBtn.BorderRadius = 15;
            BackBtn.CustomizableEdges = customizableEdges1;
            BackBtn.DisabledState.BorderColor = Color.DarkGray;
            BackBtn.DisabledState.CustomBorderColor = Color.DarkGray;
            BackBtn.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            BackBtn.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            BackBtn.FillColor = Color.FromArgb(19, 20, 42);
            BackBtn.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BackBtn.ForeColor = Color.FromArgb(142, 144, 166);
            BackBtn.HoverState.ForeColor = Color.White;
            BackBtn.HoverState.Image = Properties.Resources.backIcon;
            BackBtn.Image = Properties.Resources.darkbackIcon;
            BackBtn.Location = new Point(0, 0);
            BackBtn.Name = "BackBtn";
            BackBtn.ShadowDecoration.CustomizableEdges = customizableEdges2;
            BackBtn.Size = new Size(172, 40);
            BackBtn.TabIndex = 1;
            BackBtn.Text = "Back to folders";
            BackBtn.Click += BackBtn_Click;
            // 
            // rowPanel
            // 
            rowPanel.BackColor = Color.Transparent;
            rowPanel.BorderColor = Color.FromArgb(39, 40, 64);
            rowPanel.BorderRadius = 20;
            rowPanel.BorderThickness = 1;
            rowPanel.Controls.Add(guna2HtmlLabel2);
            rowPanel.Controls.Add(guna2HtmlLabel1);
            rowPanel.CustomizableEdges = customizableEdges3;
            rowPanel.FillColor = Color.FromArgb(26, 28, 46);
            rowPanel.Location = new Point(0, 46);
            rowPanel.Name = "rowPanel";
            rowPanel.ShadowDecoration.CustomizableEdges = customizableEdges4;
            rowPanel.Size = new Size(1192, 104);
            rowPanel.TabIndex = 2;
            rowPanel.UseTransparentBackground = true;
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Font = new Font("League Spartan", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel2.ForeColor = Color.FromArgb(142, 144, 166);
            guna2HtmlLabel2.Location = new Point(22, 47);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(159, 28);
            guna2HtmlLabel2.TabIndex = 2;
            guna2HtmlLabel2.Text = "3 emails in this thread";
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("League Spartan SemiBold", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.White;
            guna2HtmlLabel1.Location = new Point(22, 16);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(539, 37);
            guna2HtmlLabel1.TabIndex = 1;
            guna2HtmlLabel1.Text = "Enrollment Test Subject etc. CS 2202 - Kury Sebastian";
            guna2HtmlLabel1.Click += guna2HtmlLabel1_Click;
            // 
            // ECPanel
            // 
            ECPanel.AutoScroll = true;
            ECPanel.BackColor = Color.FromArgb(14, 15, 20);
            ECPanel.FlowDirection = FlowDirection.TopDown;
            ECPanel.Location = new Point(0, 156);
            ECPanel.Name = "ECPanel";
            ECPanel.Size = new Size(1218, 444);
            ECPanel.TabIndex = 3;
            ECPanel.WrapContents = false;
            ECPanel.Paint += flowLayoutPanel1_Paint;
            // 
            // guna2vScrollBar1
            // 
            guna2vScrollBar1.BindingContainer = ECPanel;
            guna2vScrollBar1.BorderRadius = 5;
            guna2vScrollBar1.InUpdate = false;
            guna2vScrollBar1.LargeChange = 10;
            guna2vScrollBar1.Location = new Point(1197, 156);
            guna2vScrollBar1.Name = "guna2vScrollBar1";
            guna2vScrollBar1.RightToLeft = RightToLeft.No;
            guna2vScrollBar1.ScrollbarSize = 21;
            guna2vScrollBar1.Size = new Size(21, 444);
            guna2vScrollBar1.TabIndex = 0;
            // 
            // EmailContent
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(14, 15, 20);
            Controls.Add(guna2vScrollBar1);
            Controls.Add(ECPanel);
            Controls.Add(rowPanel);
            Controls.Add(BackBtn);
            Name = "EmailContent";
            Size = new Size(1221, 600);
            rowPanel.ResumeLayout(false);
            rowPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button BackBtn;
        private Guna.UI2.WinForms.Guna2Panel rowPanel;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private FlowLayoutPanel ECPanel;
        private Guna.UI2.WinForms.Guna2VScrollBar guna2vScrollBar1;
    }
}
