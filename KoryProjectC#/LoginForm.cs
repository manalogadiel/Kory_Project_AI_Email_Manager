using Guna.UI2.WinForms;

namespace KoryProjectC_
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
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
    }
}