using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class EmailRow : UserControl
    {
        private readonly Color normalColor = Color.FromArgb(26, 28, 46);
        private readonly Color hoverColor = Color.FromArgb(17, 18, 30);
        private readonly Color normalBorder = Color.FromArgb(39, 40, 64); // or whatever original border color
        private readonly Color hoverBorder = Color.FromArgb(43, 40, 89);
        public EmailRow()
        {
            InitializeComponent();
            AttachEvents(this);
        }

        // Attach events recursively so child labels/pictureboxes also trigger hover/click
        private void AttachEvents(Control control)
        {
            control.MouseEnter += OnHoverEnter;
            control.MouseLeave += OnHoverLeave;
            control.MouseClick += OnRowClick;
            foreach (Control child in control.Controls)
                AttachEvents(child);
        }

        private void OnHoverEnter(object? sender, EventArgs e)
        {
            rowPanel.FillColor = hoverColor;
            rowPanel.BorderColor = hoverBorder;
            this.Cursor = Cursors.Hand;
        }

        private void OnHoverLeave(object? sender, EventArgs e)
        {
            // Only reset if the mouse actually left the whole UserControl
            if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
            {
                rowPanel.FillColor = normalColor;
                rowPanel.BorderColor = normalBorder;
                this.Cursor = Cursors.Default;
            }
        }

        private Home GetHomeForm()
        {
            Control? current = this.Parent;
            while (current != null)
            {
                if (current is Home home)
                    return home;
                current = current.Parent;
            }
            return null;
        }

        private void OnRowClick(object? sender, MouseEventArgs e)
        {
            Home home = this.FindForm() as Home;
            if (home != null)
            {
                Compose compose = new Compose();
                home.ShowFullscreenCompose(compose);
            }
        }
    }
}
