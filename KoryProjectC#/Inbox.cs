using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class Inbox : UserControl
    {
        // This dictionary remembers the original Y for every panel automatically
        private Dictionary<string, int> originalPositions = new Dictionary<string, int>();
        private Control? activeControl;
        private int currentTargetY;

        public Inbox()
        {
            InitializeComponent();
            // Ensure the timer is ready
            animationTimer.Interval = 10;
            animationTimer.Enabled = false;
        }

        private void ResetAllOtherPanels(Control currentPanel)
        {
            // This looks through your main container (guna2Panel1) 
            // and finds any panel that isn't the one you just touched.
            foreach (Control ctrl in guna2Panel1.Controls)
            {
                if (ctrl is Guna.UI2.WinForms.Guna2Panel && ctrl != currentPanel)
                {
                    if (originalPositions.ContainsKey(ctrl.Name))
                    {
                        // Snap it back to its original Y immediately
                        ctrl.Location = new Point(ctrl.Location.X, originalPositions[ctrl.Name]);
                    }
                }
            }
        }

        private void category_MouseEnter(object sender, EventArgs e)
        {
            activeControl = sender as Control;
            if (activeControl == null) return;

            // Save the original position the very first time we touch this panel
            if (!originalPositions.ContainsKey(activeControl.Name))
            {
                originalPositions[activeControl.Name] = activeControl.Location.Y;
            }

            // FIX: Snap others down so they don't stay "stuck"
            ResetAllOtherPanels(activeControl);

            currentTargetY = originalPositions[activeControl.Name] - 10;
            animationTimer.Start();
        }

        private void category_MouseLeave(object sender, EventArgs e)
        {
            Control? hoveredControl = sender as Control;
            if (hoveredControl == null) return;

            // Check if mouse is actually outside the panel bounds
            if (!hoveredControl.ClientRectangle.Contains(hoveredControl.PointToClient(Cursor.Position)))
            {
                activeControl = hoveredControl;
                if (originalPositions.ContainsKey(activeControl.Name))
                {
                    currentTargetY = originalPositions[activeControl.Name];
                    animationTimer.Start();
                }
            }
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            if (activeControl == null) return;

            int currentY = activeControl.Location.Y;
            int speed = 2; // Slightly faster for responsiveness

            if (currentY > currentTargetY) // Move Up
            {
                activeControl.Location = new Point(activeControl.Location.X, Math.Max(currentTargetY, currentY - speed));
            }
            else if (currentY < currentTargetY) // Move Down
            {
                activeControl.Location = new Point(activeControl.Location.X, Math.Min(currentTargetY, currentY + speed));
            }
            else
            {
                animationTimer.Stop();
            }
        }

        // Keep these empty to satisfy the Designer references
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
        private void category1_Paint(object sender, PaintEventArgs e) { }
        private void guna2Button1_Click(object sender, EventArgs e) { }
        private void guna2Button2_Click(object sender, EventArgs e) { }
        private void guna2ImageButton1_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
    }
}