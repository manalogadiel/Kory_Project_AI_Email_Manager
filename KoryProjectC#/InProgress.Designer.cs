namespace KoryProjectC_
{
    partial class InProgress
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2vScrollBar1 = new Guna.UI2.WinForms.Guna2VScrollBar();
            flowLayoutPanel1 = new FlowLayoutPanel();
            rowPanel = new Guna.UI2.WinForms.Guna2Panel();
            RefreshBtn = new Guna.UI2.WinForms.Guna2Button();
            rowPanel.SuspendLayout();
            SuspendLayout();
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Font = new Font("League Spartan", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel2.ForeColor = Color.FromArgb(142, 144, 166);
            guna2HtmlLabel2.Location = new Point(28, 50);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(268, 28);
            guna2HtmlLabel2.TabIndex = 2;
            guna2HtmlLabel2.Text = "Draft emails will appear in this thread";
            guna2HtmlLabel2.Click += guna2HtmlLabel2_Click;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("League Spartan SemiBold", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.White;
            guna2HtmlLabel1.Location = new Point(28, 16);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(183, 37);
            guna2HtmlLabel1.TabIndex = 1;
            guna2HtmlLabel1.Text = "In Progress Emails";
            // 
            // guna2vScrollBar1
            // 
            guna2vScrollBar1.BindingContainer = flowLayoutPanel1;
            guna2vScrollBar1.BorderRadius = 5;
            guna2vScrollBar1.InUpdate = false;
            guna2vScrollBar1.LargeChange = 10;
            guna2vScrollBar1.Location = new Point(1197, 111);
            guna2vScrollBar1.Name = "guna2vScrollBar1";
            guna2vScrollBar1.RightToLeft = RightToLeft.No;
            guna2vScrollBar1.ScrollbarSize = 21;
            guna2vScrollBar1.Size = new Size(21, 486);
            guna2vScrollBar1.TabIndex = 8;
            guna2vScrollBar1.Scroll += guna2vScrollBar1_Scroll;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BackColor = Color.FromArgb(14, 15, 20);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(0, 111);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1218, 486);
            flowLayoutPanel1.TabIndex = 11;
            flowLayoutPanel1.WrapContents = false;
            // 
            // rowPanel
            // 
            rowPanel.BackColor = Color.Transparent;
            rowPanel.BorderColor = Color.FromArgb(39, 40, 64);
            rowPanel.BorderRadius = 20;
            rowPanel.BorderThickness = 1;
            rowPanel.Controls.Add(RefreshBtn);
            rowPanel.Controls.Add(guna2HtmlLabel2);
            rowPanel.Controls.Add(guna2HtmlLabel1);
            rowPanel.CustomizableEdges = customizableEdges3;
            rowPanel.FillColor = Color.FromArgb(26, 28, 46);
            rowPanel.Location = new Point(2, 1);
            rowPanel.Name = "rowPanel";
            rowPanel.ShadowDecoration.CustomizableEdges = customizableEdges4;
            rowPanel.Size = new Size(1192, 104);
            rowPanel.TabIndex = 10;
            rowPanel.UseTransparentBackground = true;
            // 
            // RefreshBtn
            // 
            RefreshBtn.BackColor = Color.Transparent;
            RefreshBtn.BorderColor = Color.Silver;
            RefreshBtn.BorderRadius = 20;
            RefreshBtn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            RefreshBtn.Checked = true;
            RefreshBtn.CheckedState.FillColor = Color.FromArgb(20, 22, 39);
            RefreshBtn.CheckedState.ForeColor = Color.White;
            RefreshBtn.CheckedState.Image = Properties.Resources.refreshBtn3;
            RefreshBtn.Cursor = Cursors.Hand;
            RefreshBtn.CustomizableEdges = customizableEdges1;
            RefreshBtn.DisabledState.BorderColor = Color.DarkGray;
            RefreshBtn.DisabledState.CustomBorderColor = Color.DarkGray;
            RefreshBtn.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            RefreshBtn.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            RefreshBtn.FillColor = Color.Transparent;
            RefreshBtn.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RefreshBtn.ForeColor = Color.FromArgb(142, 144, 166);
            RefreshBtn.HoverState.FillColor = Color.FromArgb(23, 24, 29);
            RefreshBtn.HoverState.ForeColor = Color.White;
            RefreshBtn.HoverState.Image = Properties.Resources.refreshBtn;
            RefreshBtn.Image = Properties.Resources.refreshBtn2;
            RefreshBtn.Location = new Point(1113, 28);
            RefreshBtn.Name = "RefreshBtn";
            RefreshBtn.ShadowDecoration.CustomizableEdges = customizableEdges2;
            RefreshBtn.Size = new Size(47, 46);
            RefreshBtn.TabIndex = 14;
            RefreshBtn.TextAlign = HorizontalAlignment.Left;
            // 
            // InProgress
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(guna2vScrollBar1);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(rowPanel);
            Name = "InProgress";
            Size = new Size(1221, 600);
            rowPanel.ResumeLayout(false);
            rowPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2VScrollBar guna2vScrollBar1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Guna.UI2.WinForms.Guna2Panel rowPanel;
        private Guna.UI2.WinForms.Guna2Button RefreshBtn;
    }
}
