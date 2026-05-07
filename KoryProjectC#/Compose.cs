namespace KoryProjectC_
{
    public partial class Compose : Form
    {
        public Compose()
        {
            InitializeComponent();
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            // Back button click handler
            guna2Button1.Click += (s, e) =>
            {
                Home? home = Application.OpenForms.OfType<Home>().FirstOrDefault();
                home?.HideFullscreenCompose();
            };
        }

     
        private void SubjectForm_TextChanged(object sender, EventArgs e)
        {

        }

        private void EmailForm_Click(object sender, EventArgs e)
        {

        }

        private void NameForm_Click(object sender, EventArgs e)
        {

        }

        private void Guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Compose_Load(object sender, EventArgs e)
        {
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click_1(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {

        }

        private void EmailContent_Click(object sender, EventArgs e)
        {

        }

        private void ComposingWriteForm_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
