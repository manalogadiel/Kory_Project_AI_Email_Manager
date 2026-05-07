namespace KoryProjectC_
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Show login first; only open Home if login succeeds
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK)
                return;

            Application.Run(new Home());
        }
    }
}