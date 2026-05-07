using Google.Apis.Gmail.v1;

namespace KoryProjectC_
{
    public static class AppState
    {
        public static GmailService? GmailService { get; set; }
        public static List<EmailModel> Emails { get; set; } = new();
        public static string UserEmail { get; set; } = "";
    }
}