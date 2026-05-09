using Guna.UI2.WinForms;

namespace KoryProjectC_
{
    public partial class LoginForm : Form
    {
        private Image normalImage;
        private Image hoverImage;
        private System.Windows.Forms.Timer imageTimer;
        private bool showingNormal = true;
        private System.Windows.Forms.Timer bounceTimer;
        private int originalY;
        private int bounceOffset = 0;
        private int bounceDirection = -1;
        private int bounceMax = 10;
        public LoginForm()
        {
            InitializeComponent();
            normalImage = Image.FromFile("Resources\\normalImage.png");
            hoverImage = Image.FromFile("Resources\\hoverImage.png");

            originalY = guna2PictureBox1.Location.Y;
            guna2PictureBox1.Image = normalImage;
            imageTimer = new System.Windows.Forms.Timer();
            imageTimer.Interval = 1000; // 0.5 seconds
            imageTimer.Tick += ImageTimer_Tick;
            imageTimer.Start();
            bounceTimer = new System.Windows.Forms.Timer();
            bounceTimer.Interval = 20;
            bounceTimer.Tick += BounceTimer_Tick;
            bounceTimer.Start();
        }

        private async void Guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Guna2Button1.Enabled = false;
                Guna2Button1.Text = "Signing in...";

                // OAuth2 - opens browser for Google login
                var service = await GmailHelper.AuthenticateAsync();
                AppState.GmailService = service;

                Guna2Button1.Text = "Fetching emails...";

                var profile = await service.Users.GetProfile("me").ExecuteAsync();
                AppState.UserEmail = profile.EmailAddress ?? "";

                // Fetch 50 emails and store globally
                var emails = await GmailHelper.FetchEmailsAsync(service, 50);
                AppState.Emails = emails;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Login failed:\n\n{ex.Message}",
                    "Authentication Error",
                    MessageBoxButtons.OK,
                MessageBoxIcon.Error);

                Guna2Button1.Enabled = true;
                Guna2Button1.Text = "Sign in with Google";
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void ImageTimer_Tick(object sender, EventArgs e)
        {
            if (showingNormal)
            {
                guna2PictureBox1.Image = hoverImage;
                showingNormal = false;
            }
            else
            {
                guna2PictureBox1.Image = normalImage;
                showingNormal = true;
            }
        }

        private void BounceTimer_Tick(object sender, EventArgs e)
        {
            bounceOffset += bounceDirection * 1;

            if (bounceOffset <= -bounceMax) bounceDirection = 1;
            if (bounceOffset >= 0) bounceDirection = -1;

            guna2PictureBox1.Location = new Point(
                guna2PictureBox1.Location.X,
                originalY + bounceOffset
            );
        }

        private void guna2PictureBox1_MouseEnter(object sender, EventArgs e)
        {
        }

        private void guna2PictureBox1_MouseLeave(object sender, EventArgs e)
        {
        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}