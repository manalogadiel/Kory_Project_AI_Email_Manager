using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace KoryProjectC_
{
    public static class GmailHelper
    {
        private static readonly string[] Scopes =
        {
            GmailService.Scope.GmailReadonly,
            GmailService.Scope.GmailSend,
            "https://www.googleapis.com/auth/userinfo.profile"

        };

        // ── Stored credential so GetProfilePictureAsync can use the token ────────
        private static UserCredential? _credential;

        // Make sure this scope is included


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

            _credential = credential; // ← store for profile picture use

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "KoryProject"
            });
        }

        // ── NEW: Fetch the Google account profile picture ─────────────────────────
        /// <summary>
        /// Downloads the authenticated user's Google profile picture.
        /// Returns null if unavailable — picture box will just keep its default image.
        /// </summary>
        public static async Task<Image?> GetProfilePictureAsync()
        {
            if (_credential == null) return null;

            try
            {
                // Refresh token if expired
                await _credential.RefreshTokenAsync(CancellationToken.None);
                string token = _credential.Token.AccessToken;

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Fetch profile info JSON (includes picture URL)
                string json = await http.GetStringAsync(
                    "https://www.googleapis.com/oauth2/v1/userinfo");

                // Pull picture URL out of the JSON
                var match = Regex.Match(json, @"""picture""\s*:\s*""(.+?)""");
                if (!match.Success) return null;

                string url = match.Groups[1].Value.Replace("\\/", "/");

                // Download and return the image
                byte[] bytes = await http.GetByteArrayAsync(url);
                using var ms = new MemoryStream(bytes);
                return Image.FromStream(ms);
            }
            catch
            {
                return null; // silently fall back — picture box stays as default
            }
        }

        public static async Task<List<EmailModel>> FetchEmailsAsync(
            GmailService service, int maxResults = 50)
        {
            var listRequest = service.Users.Messages.List("me");
            listRequest.MaxResults = maxResults;
            listRequest.LabelIds = "INBOX";

            var listResponse = await listRequest.ExecuteAsync();
            if (listResponse.Messages == null) return new List<EmailModel>();

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

        /// <summary>
        /// Returns the number of reply emails the user has sent today.
        /// Feeds the "Answered Today" stat card.
        /// </summary>
        public static async Task<int> GetSentTodayCountAsync(GmailService service)
        {
            var startOfDay = new DateTimeOffset(DateTime.Today,
                TimeZoneInfo.Local.GetUtcOffset(DateTime.Today));
            long unixTimestamp = startOfDay.ToUnixTimeSeconds();

            var req = service.Users.Messages.List("me");
            req.LabelIds = "SENT";
            req.Q = $"after:{unixTimestamp}";
            req.MaxResults = 200;

            var resp = await req.ExecuteAsync();
            if (resp.Messages == null) return 0;

            return resp.Messages.Count;
        }

        /// <summary>
        /// Samples the last <paramref name="sampleSize"/> sent messages, finds each
        /// thread's first received message, and returns the mean response time in
        /// minutes. Feeds the "Avg. Response" stat card.
        /// Returns 0 if there is not enough data.
        /// </summary>
        public static async Task<double> GetAvgResponseMinutesAsync(
            GmailService service, int sampleSize = 20)
        {
            var sentReq = service.Users.Messages.List("me");
            sentReq.LabelIds = "SENT";
            sentReq.MaxResults = sampleSize;
            var sentResp = await sentReq.ExecuteAsync();

            if (sentResp.Messages == null || !sentResp.Messages.Any())
                return 0;

            var threadTasks = sentResp.Messages
                .Take(sampleSize)
                .Select(async msg =>
                {
                    try
                    {
                        var getReq = service.Users.Messages.Get("me", msg.Id);
                        getReq.Format = UsersResource.MessagesResource
                                            .GetRequest.FormatEnum.Minimal;
                        var sentMsg = await getReq.ExecuteAsync();

                        if (sentMsg.ThreadId == null) return (double?)null;

                        var threadReq = service.Users.Threads.Get("me", sentMsg.ThreadId);
                        threadReq.Format = UsersResource.ThreadsResource
                                               .GetRequest.FormatEnum.Minimal;
                        var thread = await threadReq.ExecuteAsync();

                        if (thread.Messages == null || thread.Messages.Count < 2)
                            return (double?)null;

                        var first = thread.Messages.First();
                        var sent = thread.Messages.FirstOrDefault(m => m.Id == sentMsg.Id);

                        if (first.InternalDate == null || sent?.InternalDate == null)
                            return (double?)null;

                        double minutes =
                            (sent.InternalDate.Value - first.InternalDate.Value) / 60_000.0;

                        if (minutes < 0 || minutes > 10_080) return (double?)null;

                        return (double?)minutes;
                    }
                    catch
                    {
                        return (double?)null;
                    }
                });

            var results = await Task.WhenAll(threadTasks);
            var validTimes = results.OfType<double>().ToList();

            return validTimes.Count > 0 ? validTimes.Average() : 0;
        }

        public static async Task<string> GetUserNameAsync(GmailService service)
        {
            // 1. Use stored credential to call userinfo endpoint (same as GetProfilePictureAsync)
            if (_credential != null)
            {
                try
                {
                    await _credential.RefreshTokenAsync(CancellationToken.None);
                    string token = _credential.Token.AccessToken;

                    using var http = new HttpClient();
                    http.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = await http.GetStringAsync(
                        "https://www.googleapis.com/oauth2/v1/userinfo");

                    var match = Regex.Match(json, @"""given_name""\s*:\s*""(.+?)""");
                    if (match.Success)
                        return match.Groups[1].Value; // → "Gadiel Gospel"
                }
                catch { }
            }

            // 2. Fallback: display name from a sent email's From header
            try
            {
                var sentReq = service.Users.Messages.List("me");
                sentReq.LabelIds = "SENT";
                sentReq.MaxResults = 1;
                var sentResp = await sentReq.ExecuteAsync();

                if (sentResp.Messages?.Any() == true)
                {
                    var getReq = service.Users.Messages.Get("me", sentResp.Messages[0].Id);
                    getReq.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Metadata;
                    getReq.MetadataHeaders = new[] { "From" };
                    var msg = await getReq.ExecuteAsync();

                    var from = msg.Payload?.Headers?
                        .FirstOrDefault(h => h.Name?.Equals("From", StringComparison.OrdinalIgnoreCase) == true);

                    if (from?.Value != null)
                    {
                        // "John Doe <john@gmail.com>" → returns "John Doe" (full name)
                        var match = Regex.Match(from.Value ?? "", @"^""?(.+?)""?\s*<");
                        if (match.Success)
                            return match.Groups[1].Value.Trim(); // removed .Split(' ')[0]
                    }
                }
            }
            catch { }

            var profile = await service.Users.GetProfile("me").ExecuteAsync();
            return (profile.EmailAddress ?? "there").Split('@')[0];
        }

        public static async Task<List<EmailModel>> FetchSentEmailsAsync(
    GmailService service, int maxResults = 50)
        {
            var startOfDay = new DateTimeOffset(DateTime.Today,
                TimeZoneInfo.Local.GetUtcOffset(DateTime.Today));
            long unixTimestamp = startOfDay.ToUnixTimeSeconds();

            var listRequest = service.Users.Messages.List("me");
            listRequest.MaxResults = maxResults;
            listRequest.LabelIds = "SENT";
            listRequest.Q = $"after:{unixTimestamp}";

            var listResponse = await listRequest.ExecuteAsync();
            if (listResponse.Messages == null) return new List<EmailModel>();

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

        // ─────────────────────────────────────────────────────────────────────────

        // ─────────────────────────────────────────────────────────────────────────
        //  PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────────────────

        private static EmailModel ParseEmail(Google.Apis.Gmail.v1.Data.Message message)
        {
            var email = new EmailModel
            {
                Id = message.Id ?? "",
                ThreadId = message.ThreadId ?? "",
                Snippet = message.Snippet ?? "",
                IsRead = !(message.LabelIds?.Contains("UNREAD") ?? false)
            };

            foreach (var h in message.Payload?.Headers
                              ?? Enumerable.Empty<Google.Apis.Gmail.v1.Data.MessagePartHeader>())
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

            var cleaned = Regex.Replace(raw, @"\s*\(.*?\)\s*$", "").Trim();

            if (!DateTime.TryParse(cleaned, null,
                    System.Globalization.DateTimeStyles.AdjustToUniversal, out var dt))
                return raw;

            dt = dt.ToLocalTime();
            var now = DateTime.Now;
            var diff = now - dt;

            if (diff.TotalDays < 1 && dt.Date == now.Date)
                return dt.ToString("h:mm tt");

            if (diff.TotalDays < 7)
                return dt.ToString("ddd, h:mm tt");

            if (diff.TotalDays < 14)
                return "A week ago";

            if (diff.TotalDays < 60)
                return "A month ago";

            if (diff.TotalDays < 365)
                return dt.ToString("MMM d");

            return dt.ToString("MMM d, yyyy");
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

        public static async Task SendReplyAsync(
            GmailService service,
            EmailModel originalEmail,
            string subject,
            string salutation,
            string body,
            string signature)
        {
            string fullBody = $"{salutation}\r\n\r\n{body}\r\n\r\n{signature}";

            string rawMessage = BuildRawReply(
                originalEmail.FromEmail,
                subject,
                fullBody,
                originalEmail.Id,
                originalEmail.ThreadId
            );

            var message = new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = rawMessage,
                ThreadId = originalEmail.ThreadId
            };

            await service.Users.Messages.Send(message, "me").ExecuteAsync();
        }

        private static string BuildRawReply(
            string to,
            string subject,
            string body,
            string inReplyToId,
            string threadId)
        {
            string fromEmail = AppState.UserEmail;
            string fromName = AppState.UserEmail; // fallback

            // Read full name from saved profile
            string profilePath = Path.Combine(Application.StartupPath, "profile.json");
            if (File.Exists(profilePath))
            {
                try
                {
                    var json = File.ReadAllText(profilePath);
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("FullName", out var fn) &&
                        !string.IsNullOrWhiteSpace(fn.GetString()))
                        fromName = fn.GetString()!;
                }
                catch { }
            }

            string mime =
                $"To: {to}\r\n" +
                $"From: {fromName} <{fromEmail}>\r\n" +
                $"Subject: {subject}\r\n" +
                $"In-Reply-To: {inReplyToId}\r\n" +
                $"References: {inReplyToId}\r\n" +
                $"Content-Type: text/plain; charset=utf-8\r\n" +
                $"\r\n" +
                $"{body}";

            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(mime))
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
        private static bool Has(string text, params string[] kw)
            => kw.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
        public static async Task SendNewEmailAsync(
             GmailService service,
             string to,
             string subject,
             string body)
                    {
                     string fromName = AppState.UserName;
                     string fromEmail = AppState.UserEmail;

                      string mime =
                            $"To: {to}\r\n" +
                            $"From: {fromName} <{fromEmail}>\r\n" +
                            $"Subject: {subject}\r\n" +
                            $"Content-Type: text/plain; charset=utf-8\r\n" +
                            $"\r\n" +
                            $"{body}";

                        var message = new Google.Apis.Gmail.v1.Data.Message
                        {
                            Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(mime))
                                .Replace('+', '-')
                                .Replace('/', '_')
                                .TrimEnd('=')
                        };

                        await service.Users.Messages.Send(message, "me").ExecuteAsync();
                    }
    }
}