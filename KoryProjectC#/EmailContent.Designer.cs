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
            BackBtn = new Guna.UI2.WinForms.Guna2Button();
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
            BackBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BackBtn.ForeColor = Color.FromArgb(142, 144, 166);
            BackBtn.HoverState.ForeColor = Color.White;
            BackBtn.HoverState.Image = Properties.Resources.backIcon;
            BackBtn.Image = Properties.Resources.darkbackIcon;
            BackBtn.Location = new Point(52, 44);
            BackBtn.Name = "BackBtn";
            BackBtn.ShadowDecoration.CustomizableEdges = customizableEdges2;
            BackBtn.Size = new Size(159, 40);
            BackBtn.TabIndex = 1;
            BackBtn.Text = "Back to Inbox";
            BackBtn.Click += this.BackBtn_Click;
            // 
            // EmailContent
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(BackBtn);
            Name = "EmailContent";
            Size = new Size(1192, 600);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button BackBtn;
    }
}
