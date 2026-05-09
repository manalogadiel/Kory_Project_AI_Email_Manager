using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class logOutConfirmation : Form
    {
        public bool Confirmed { get; private set; } = false;

        public logOutConfirmation()
        {
            InitializeComponent();
        }

        private void title_Click(object sender, EventArgs e) { }

        private void logOutYES_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            this.Close();
        }

        private void logOutNo_Click(object sender, EventArgs e)
        {
            Confirmed = false;
            this.Close();
        }

        private void logOutConfirmation_Load(object sender, EventArgs e) { }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}