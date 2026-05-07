namespace KoryProjectC_
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // TODO: Replace this with your logic to check if a user is saved/logged in
            bool isUserLoggedIn = false;

            if (!isUserLoggedIn)
            {
                // 1. Create the Login Form (Change 'LoginForm' if your form has a different name)
                LoginForm login = new LoginForm();

                // 2. Show it as a dialog and wait for the result
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // 3. Login was successful! Now we can run your Home form.
                    Application.Run(new Home());
                }
                else
                {
                    // The user closed the login form without logging in.
                    // The application will simply end here.
                    return;
                }
            }
            else
            {
                // The user was already logged in, skip straight to the Home form
                Application.Run(new Home());
            }
        }
    }
}