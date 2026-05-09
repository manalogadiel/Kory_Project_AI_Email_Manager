using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class logOutConfirmation : Form
    {
        public bool Confirmed { get; private set; } = false;
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect,
        int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);

        public logOutConfirmation()
        {
            InitializeComponent();
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
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

        private void logOutConfirmation_Load(object sender, EventArgs e)
        {
            // Reapply region in case size changed
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));




        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }
    }
}