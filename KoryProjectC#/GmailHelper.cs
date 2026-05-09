using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Text.RegularExpressions;

namespace KoryProjectC_
{
    public static class GmailHelper
    {
        private static readonly string[] Scopes =
        {
            GmailService.Scope.GmailReadonly,
            GmailService.Scope.GmailSend
        };

        public static async Task<GmailService> AuthenticateAsync()
        {
            string credPath = Path.Combine(Application.StartupPath, "credentials.json");

            if (!File.Exists(credPath))
                throw new FileNotFoundException(
                    "credentials.json not found.\n" +
                    "Download it from Google Cloud Console and place it in the app folder.");

            UserCredential credential;
            using var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read);

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(
                    Path.Combine(Application.StartupPath, "token_store"), true));

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "KoryProject"
            });
        }

        public static async Task<List<EmailModel>> FetchEmailsAsync(
            GmailService service, int maxResults = 50)
        {
            var listRequest = service.Users.Messages.List("me");
            listRequest.MaxResults = maxResults;
            listRequest.LabelIds = "INBOX";

            var listResponse = await listRequest.ExecuteAsync();
            if (listResponse.Messages == null) return new List<EmailModel>();

            // Fetch all in parallel
            var tasks = listResponse.Messages.Select(async msg =>
            {
                var req = service.Users.Messages.Get("me", msg.Id);
                req.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                var message = await req.ExecuteAsync();
                return ParseEmail(message);
            });

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        private static EmailModel ParseEmail(Google.Apis.Gmail.v1.Data.Message message)
        {
            var email = new EmailModel
            {
                Id = message.Id ?? "",
                ThreadId = message.ThreadId ?? "",
                Snippet = message.Snippet ?? "",
                IsRead = !(message.LabelIds?.Contains("UNREAD") ?? false)
            };

            foreach (var h in message.Payload?.Headers ?? Enumerable.Empty<Google.Apis.Gmail.v1.Data.MessagePartHeader>())
            {
                switch (h.Name?.ToLower())
                {
                    case "subject":
                        email.Subject = h.Value ?? "(No Subject)";
                        break;
                    case "from":
                        ParseFrom(h.Value ?? "", out var name, out var addr);
                        email.FromName = name;
                        email.FromEmail = addr;
                        break;
                    case "date":
                        email.Date = FormatDate(h.Value);
                        break;
                }
            }

            email.BodyHtml = GetBody(message.Payload, "text/html");
            email.BodyText = GetBody(message.Payload, "text/plain");
            email.Category = CategorizeEmail(email.Subject, email.Snippet + " " + email.BodyText);

            return email;
        }

        private static void ParseFrom(string from, out string name, out string email)
        {
            var match = Regex.Match(from, @"^""?(.+?)""?\s*<(.+?)>$");
            if (match.Success)
            {
                name = match.Groups[1].Value.Trim();
                email = match.Groups[2].Value.Trim();
            }
            else
            {
                name = from.Trim();
                email = from.Trim();
            }
        }

        private static string GetBody(
            Google.Apis.Gmail.v1.Data.MessagePart? part, string mimeType)
        {
            if (part == null) return "";

            if (part.MimeType == mimeType && part.Body?.Data != null)
                return DecodeBase64Url(part.Body.Data);

            if (part.Parts != null)
                foreach (var sub in part.Parts)
                {
                    var result = GetBody(sub, mimeType);
                    if (!string.IsNullOrEmpty(result)) return result;
                }

            return "";
        }

        private static string DecodeBase64Url(string data)
        {
            var b64 = data.Replace('-', '+').Replace('_', '/');
            b64 = b64.PadRight(b64.Length + (4 - b64.Length % 4) % 4, '=');
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(b64));
        }

        private static string FormatDate(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";

            // Gmail sends RFC 2822 dates like "Thu, 30 Apr 2026 01:03:17 +0000 (UTC)"
            // Strip the timezone label in parentheses so TryParse can handle it
            var cleaned = Regex.Replace(raw, @"\s*\(.*?\)\s*$", "").Trim();

            if (!DateTime.TryParse(cleaned, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var dt))
                return raw;

            dt = dt.ToLocalTime();
            var now = DateTime.Now;
            var diff = now - dt;

            if (diff.TotalDays < 1 && dt.Date == now.Date)
                return dt.ToString("h:mm tt");         // 2:42 PM

            if (diff.TotalDays < 7)
                return dt.ToString("ddd, h:mm tt");    // Mon, 12:12 AM

            if (diff.TotalDays < 14)
                return "A week ago";

            if (diff.TotalDays < 60)
                return "A month ago";

            if (diff.TotalDays < 365)
                return dt.ToString("MMM d");           // Apr 30

            return dt.ToString("MMM d, yyyy");         // Apr 30, 2025
        }


        public static string CategorizeEmail(string subject, string body)
        {
            var text = (subject + " " + body).ToLower();

            if (Has(text, "grade", "score", "exam", "quiz", "midterm", "final grade",
                          "grade inquiry", "gwa", "failing", "rating"))
                return "GRADE CONCERNS";

            if (Has(text, "absent", "excuse", "excused", "sick", "attendance",
                          "late", "tardy", "missed class"))
                return "ABSENTS / EXCUSES";

            if (Has(text, "request", "requesting", "kindly", "please allow",
                          "permission", "petition"))
                return "REQUESTS";

            if (Has(text, "concern", "academic", "enrollment", "dropped",
                          "withdraw", "appeal", "complaint"))
                return "ACADEMIC CONCERNS";

            if (Has(text, "requirement", "submit", "submission", "deadline",
                          "clearance", "document", "form", "certificate", "assignment", "project"))
                return "REQUIREMENTS";

            return "NON-ACADEMIC";
        }

        private static bool Has(string text, params string[] kw)
            => kw.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}