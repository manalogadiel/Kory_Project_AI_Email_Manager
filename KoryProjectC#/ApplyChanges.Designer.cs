namespace KoryProjectC_
{
    partial class ApplyChanges
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplyChanges));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            applyChangesLabel = new Guna.UI2.WinForms.Guna2TextBox();
            SuspendLayout();
            // 
            // applyChangesLabel
            // 
            applyChangesLabel.AutoScroll = true;
            applyChangesLabel.BorderThickness = 0;
            applyChangesLabel.CustomizableEdges = customizableEdges1;
            applyChangesLabel.DefaultText = resources.GetString("applyChangesLabel.DefaultText");
            applyChangesLabel.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            applyChangesLabel.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            applyChangesLabel.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            applyChangesLabel.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            applyChangesLabel.FillColor = Color.FromArgb(15, 16, 32);
            applyChangesLabel.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            applyChangesLabel.Font = new Font("Segoe UI", 9F);
            applyChangesLabel.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            applyChangesLabel.Location = new Point(34, 32);
            applyChangesLabel.Margin = new Padding(3, 4, 3, 4);
            applyChangesLabel.MaximumSize = new Size(700, 350);
            applyChangesLabel.Multiline = true;
            applyChangesLabel.Name = "applyChangesLabel";
            applyChangesLabel.PlaceholderText = "";
            applyChangesLabel.SelectedText = "";
            applyChangesLabel.ShadowDecoration.CustomizableEdges = customizableEdges2;
            applyChangesLabel.Size = new Size(700, 350);
            applyChangesLabel.TabIndex = 0;
            // 
            // ApplyChanges
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 16, 32);
            ClientSize = new Size(758, 518);
            Controls.Add(applyChangesLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "ApplyChanges";
            Text = "ApplyChanges";
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2TextBox applyChangesLabel;
    }
}