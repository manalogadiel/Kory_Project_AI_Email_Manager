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
    public partial class SingleCompose : Form
    {
        public SingleCompose()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent; // centers over Home
            this.FormBorderStyle = FormBorderStyle.FixedDialog;  // no resize
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void SingleCompose_Load(object sender, EventArgs e)
        {

        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
