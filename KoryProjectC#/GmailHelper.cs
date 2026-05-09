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

        private static UserCredential? _credential;

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

            _credential = credential;

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "KoryProject"
            });
        }

        public static async Task<Image?> GetProfilePictureAsync()
        {
            if (_credential == null) return null;

            try
            {
                await _credential.RefreshTokenAsync(CancellationToken.None);
                string token = _credential.Token.AccessToken;

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                string json = await http.GetStringAsync(
                    "https://www.googleapis.com/oauth2/v1/userinfo");

                var match = Regex.Match(json, @"""picture""\s*:\s*""(.+?)""");
                if (!match.Success) return null;

                string url = match.Groups[1].Value.Replace("\\/", "/");
                byte[] bytes = await http.GetByteArrayAsync(url);
                using var ms = new MemoryStream(bytes);
                return Image.FromStream(ms);
            }
            catch { return null; }
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
                return ParseEmail(await req.ExecuteAsync());
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        public static async Task<int> GetSentTodayCountAsync(GmailService service)
        {
            var startOfDay = new DateTimeOffset(DateTime.Today,
                TimeZoneInfo.Local.GetUtcOffset(DateTime.Today));
            long unix = startOfDay.ToUnixTimeSeconds();

            var req = service.Users.Messages.List("me");
            req.LabelIds = "SENT";
            req.Q = $"after:{unix}";
            req.MaxResults = 200;

            var resp = await req.ExecuteAsync();
            return resp.Messages?.Count ?? 0;
        }

        public static async Task<double> GetAvgResponseMinutesAsync(
    GmailService service, int sampleSize = 20)
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

            double hoursElapsed = (DateTime.Now - DateTime.Today).TotalHours;
            if (hoursElapsed < 0.1) return 0;

            return Math.Round(resp.Messages.Count / hoursElapsed, 1);
        }

        public static async Task<string> GetUserNameAsync(GmailService service)
        {
            if (_credential != null)
            {
                try
                {
                    await _credential.RefreshTokenAsync(CancellationToken.None);
                    string token = _credential.Token.AccessToken;

                    using var http = new HttpClient();
                    http.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    string json = await http.GetStringAsync("https://www.googleapis.com/oauth2/v1/userinfo");
                    var match = Regex.Match(json, @"""given_name""\s*:\s*""(.+?)""");
                    if (match.Success) return match.Groups[1].Value;
                }
                catch { }
            }

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
                        .FirstOrDefault(h => h.Name?.Equals("From",
                            StringComparison.OrdinalIgnoreCase) == true);

                    if (from?.Value != null)
                    {
                        var match = Regex.Match(from.Value, @"^""?(.+?)""?\s*<");
                        if (match.Success) return match.Groups[1].Value.Trim();
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
            long unix = startOfDay.ToUnixTimeSeconds();

            var listRequest = service.Users.Messages.List("me");
            listRequest.MaxResults = maxResults;
            listRequest.LabelIds = "SENT";
            listRequest.Q = $"after:{unix}";

            var listResponse = await listRequest.ExecuteAsync();
            if (listResponse.Messages == null) return new List<EmailModel>();

            var tasks = listResponse.Messages.Select(async msg =>
            {
                var req = service.Users.Messages.Get("me", msg.Id);
                req.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                return ParseEmail(await req.ExecuteAsync());
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        // ═════════════════════════════════════════════════════════════════════
        //  SMART CATEGORIZATION — two-layer system
        //
        //  Layer 1  Automated / marketing filter → always NON-ACADEMIC
        //           Only blocks clearly non-human senders + unmistakable
        //           marketing body patterns. Skips body check entirely for
        //           educational-domain senders (.edu, .edu.ph, batstate-u).
        //           Body is stripped of quoted reply content before checking,
        //           so footer boilerplate in forwarded threads doesn't trip it.
        //
        //  Layer 2  Weighted keyword scoring
        //           Score ≥ 3 required to qualify for a category.
        //           Filipino academic markers (po, sir, ma'am, good day)
        //           add a bonus so genuine student emails always clear the bar.
        // ═════════════════════════════════════════════════════════════════════

        public static string CategorizeEmail(string subject, string body,
                                             string fromEmail = "")
        {
            // Strip quoted reply content so footer boilerplate from older
            // messages in the thread doesn't pollute the filter or scorer.
            string cleanBody = StripQuotedContent(body);

            var text = (subject + " " + cleanBody).ToLower();
            var from = fromEmail.ToLower();

            // Layer 1 — reject obvious junk / automated emails
            if (IsAutomatedOrMarketing(from, text))
                return "NON-ACADEMIC";

            // Layer 2 — score every academic category
            // Filipino academic writing bonus: "po", "sir/ma'am", "good day" etc.
            int bonus = FilipinoAcademicBonus(text);

            var scores = new Dictionary<string, int>
            {
                ["GRADE CONCERNS"] = ScoreGradeConcerns(text) + bonus,
                ["ABSENTS / EXCUSES"] = ScoreAbsentsExcuses(text) + bonus,
                ["REQUESTS"] = ScoreRequests(text) + bonus,
                ["ACADEMIC CONCERNS"] = ScoreAcademicConcerns(text) + bonus,
                ["REQUIREMENTS"] = ScoreRequirements(text) + bonus,
            };

            // Minimum score of 3 to avoid single-generic-word false positives
            const int MinScore = 3;

            var best = scores
                .Where(kv => kv.Value >= MinScore)
                .OrderByDescending(kv => kv.Value)
                .FirstOrDefault();

            return best.Key ?? "NON-ACADEMIC";
        }

        // ── Strip quoted reply content ────────────────────────────────────────
        // Removes everything from common reply/forward markers onward so that
        // footer boilerplate in older messages of the thread can't trigger the
        // automated filter or skew keyword scores.

        private static string StripQuotedContent(string body)
        {
            if (string.IsNullOrEmpty(body)) return body;

            // Ordered from most specific to least so we cut at the earliest marker
            string[] cutoffMarkers =
            {
                "-----original message-----",
                "-----forwarded message-----",
                "\r\non ", "\non ",          // "On Mon, Jan 1 ... wrote:"
                "\r\n> ",  "\n> ",           // "> quoted line"
                "\r\n--\r\n", "\n--\n",      // standard email sig separator
            };

            int cutAt = body.Length;
            foreach (var marker in cutoffMarkers)
            {
                int idx = body.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx > 0 && idx < cutAt)
                    cutAt = idx;
            }

            return body[..cutAt].Trim();
        }

        // ── Layer 1: Automated / marketing detection ──────────────────────────

        private static bool IsAutomatedOrMarketing(string from, string text)
        {
            // ── Academic platform whitelist — always pass through to scoring ──
            // These senders are automated but carry genuine academic content
            // (assignment notifications, grade posts, class announcements).
            // Must be checked BEFORE any domain or body-pattern blocks.
            bool isAcademicPlatform =
                from.Contains("classroom.google.com") ||
                from.Contains("@classroom.google.com") ||
                from.Contains("googleclassroom") ||
                from.Contains("moodle") ||
                from.Contains("blackboard") ||
                from.Contains("canvas.instructure") ||
                from.Contains("edmodo") ||
                from.Contains("schoology");
            if (isAcademicPlatform) return false;

            // If from an educational domain → only flag the most obvious no-reply
            // senders and skip body-content checks entirely.
            bool isEducational = from.Contains(".edu") ||
                                 from.Contains("batstate-u") ||
                                 from.Contains("school") ||
                                 from.Contains("university") ||
                                 from.Contains("college");

            // Strict no-reply patterns — blocked regardless of domain
            string[] strictNoReply =
            {
                "no-reply", "noreply", "donotreply", "do-not-reply", "do_not_reply"
            };
            if (strictNoReply.Any(s => from.Contains(s))) return true;

            // For educational senders, stop here — don't run the broad body check
            if (isEducational) return false;

            // Known external service / marketing sender domains.
            // Use "canva.com" without a leading @ to catch all subdomains
            // (mail.canva.com, e.canva.com, notifications.canva.com, etc.).
            string[] externalDomains =
            {
                "canva.com", "@github.com",
                "@linkedin.com", "@facebook.com", "@twitter.com",
                "@zoom.us", "@slack.com", "@spotify.com", "@netflix.com",
                "notify@", "alert@", "auto@",
                "automated@", "mailer@", "bounce@", "postmaster@",
                "robot@", "system@"
                // NOTE: "notifications@" and "@googlegroups.com" removed —
                // Google Classroom uses these and its emails are academic.
            };
            if (externalDomains.Any(s => from.Contains(s))) return true;

            // Unmistakable automated / promotional body patterns.
            // NOTE: Kept intentionally narrow — broad terms like "privacy policy",
            // "view online", and "newsletter" were removed because they appear
            // legitimately in student email threads.
            string[] automatedBody =
            {
                "unsubscribe",
                "view this email in your browser",  // full phrase only (was "view in browser")
                "email preferences",
                "manage your notifications",
                "manage preferences",
                "opt out", "opt-out",

                // Automated-response markers
                "this is an automated",
                "this email was sent automatically",
                "do not reply to this email",
                "do not reply directly",
                "please do not reply to this",
                "this is a no-reply",

                // Google Forms / survey responses
                "a response has been submitted",
                "someone responded to",
                "you received a response",
                "your response has been recorded",
                "thanks for filling",
                "thank you for filling",
                "someone has filled out",

                // Canva / design tool marketing — body-level fallback for when
                // the sender routes through a subdomain that slips the domain check
                "start designing",
                "create a design",
                "make your designs",
                "tips to make your designs",
                "your designs shine",
                "design with canva",
                "ready to design",

                // Newsletters / promos — only the unambiguous patterns
                "special offer",
                "limited time offer",
                "you're receiving this because",
                "you received this email because",
                "sent to you because",
            };
            if (automatedBody.Any(p => text.Contains(p))) return true;

            return false;
        }

        // ── Filipino academic writing bonus ───────────────────────────────────
        // Student emails in PH almost always contain these phrases.
        // Adding a bonus means even borderline emails from real students
        // will clear the minimum-score threshold.

        private static int FilipinoAcademicBonus(string text)
        {
            int bonus = 0;

            // Honorifics / politeness markers
            if (Regex.IsMatch(text, @"\bpo\b")) bonus += 2; // "po" as standalone word
            if (text.Contains("opo")) bonus += 2;
            if (text.Contains("good day")) bonus += 1;
            if (text.Contains("good morning")) bonus += 1;
            if (text.Contains("good afternoon")) bonus += 1;

            // Salutation patterns
            if (text.Contains("dear sir")) bonus += 2;
            if (text.Contains("dear ma'am") ||
                text.Contains("dear maam")) bonus += 2;
            if (text.Contains("dear prof")) bonus += 2;
            if (text.Contains("dear professor")) bonus += 2;
            if (text.Contains("dear teacher")) bonus += 2;
            if (text.Contains("dear instructor")) bonus += 2;

            // Respectful closings
            if (text.Contains("respectfully")) bonus += 1;
            if (text.Contains("your student")) bonus += 2;
            if (text.Contains("humbly")) bonus += 1;

            return bonus;
        }

        // ── Layer 2: Weighted keyword scoring ────────────────────────────────
        //
        //  3 pts — highly specific academic phrase
        //  2 pts — moderately specific term
        //  1 pt  — generic word (needs combination to reach threshold)

        private static int ScoreGradeConcerns(string text)
        {
            int s = 0;

            // Highly specific phrases (+3)
            if (text.Contains("grade inquiry")) s += 3;
            if (text.Contains("final grade")) s += 3;
            if (text.Contains("gwa")) s += 3;
            if (text.Contains("grade point")) s += 3;
            if (text.Contains("class standing")) s += 3;
            if (text.Contains("passing grade")) s += 3;
            if (text.Contains("incomplete grade")) s += 3;
            if (text.Contains("academic standing")) s += 3;
            if (text.Contains("transmutation")) s += 3;
            if (text.Contains("computed grade")) s += 3;
            if (text.Contains("encoded grade")) s += 3;
            if (text.Contains("midterm exam")) s += 3;   // "graded midterm exam" now scores +3 here...
            if (text.Contains("quiz score")) s += 3;
            if (text.Contains("exam result")) s += 3;
            if (text.Contains("exam grade")) s += 3;
            if (text.Contains("prelim exam")) s += 3;
            if (text.Contains("final exam")) s += 3;
            if (text.Contains("long exam")) s += 3;

            // Moderately specific (+2)
            if (text.Contains("midterm")) s += 2;
            if (text.Contains("finals")) s += 2;
            if (text.Contains("failing")) s += 2;
            if (text.Contains("failed")) s += 2;
            if (text.Contains("quiz")) s += 2;
            if (text.Contains("exam")) s += 2;           // ...and +2 here, giving a combined +5 minimum
            if (text.Contains("rating")) s += 2;
            if (text.Contains("prelim")) s += 2;
            if (text.Contains("graded")) s += 2;         // "graded" alone is a strong signal

            // Generic — need combination (+1)
            if (text.Contains("grade")) s += 1;
            if (text.Contains("score")) s += 1;
            if (text.Contains("marks")) s += 1;
            if (text.Contains("result")) s += 1;

            return s;
        }

        private static int ScoreAbsentsExcuses(string text)
        {
            int s = 0;

            // Highly specific (+3)
            if (text.Contains("excused absence")) s += 3;
            if (text.Contains("excuse letter")) s += 3;
            if (text.Contains("missed class")) s += 3;
            if (text.Contains("class absence")) s += 3;
            if (text.Contains("attendance record")) s += 3;
            if (text.Contains("absent on")) s += 3;
            if (text.Contains("medical certificate") &&
                text.Contains("absent")) s += 3;

            // Moderately specific (+2)
            if (text.Contains("excused")) s += 2;
            if (text.Contains("tardy")) s += 2;
            if (text.Contains("attendance")) s += 2;
            if (text.Contains("missed")) s += 2;
            if (text.Contains("sick leave")) s += 2;
            if (text.Contains("medical")) s += 2;

            // Generic (+1)
            if (text.Contains("absent")) s += 1;
            if (text.Contains("late")) s += 1;
            if (text.Contains("excuse")) s += 1;

            return s;
        }

        private static int ScoreRequests(string text)
        {
            int s = 0;

            // Highly specific (+3)
            if (text.Contains("kindly allow")) s += 3;
            if (text.Contains("please allow")) s += 3;
            if (text.Contains("humbly request")) s += 3;
            if (text.Contains("respectfully request")) s += 3;
            if (text.Contains("i am requesting")) s += 3;
            if (text.Contains("i would like to request")) s += 3;
            if (text.Contains("grant permission")) s += 3;
            if (text.Contains("special permission")) s += 3;
            if (text.Contains("your permission")) s += 3;

            // Moderately specific (+2)
            if (text.Contains("petition")) s += 2;
            if (text.Contains("requesting")) s += 2;
            if (text.Contains("permission")) s += 2;
            if (text.Contains("kindly")) s += 2;

            // Generic (+1)
            if (text.Contains("request")) s += 1;

            return s;
        }

        private static int ScoreAcademicConcerns(string text)
        {
            int s = 0;

            // Highly specific (+3)
            if (text.Contains("academic concern")) s += 3;
            if (text.Contains("dropped subject")) s += 3;
            if (text.Contains("withdrawal form")) s += 3;
            if (text.Contains("shifting to")) s += 3;
            if (text.Contains("change of course")) s += 3;
            if (text.Contains("academic appeal")) s += 3;
            if (text.Contains("formal complaint")) s += 3;
            if (text.Contains("academic dismissal")) s += 3;
            if (text.Contains("probationary")) s += 3;
            if (text.Contains("irregular student")) s += 3;

            // Moderately specific (+2)
            if (text.Contains("enrollment")) s += 2;
            if (text.Contains("withdraw")) s += 2;
            if (text.Contains("appeal")) s += 2;
            if (text.Contains("complaint")) s += 2;
            if (text.Contains("academic")) s += 2;
            if (text.Contains("dropped")) s += 2;

            // Generic (+1)
            if (text.Contains("concern")) s += 1;

            return s;
        }

        private static int ScoreRequirements(string text)
        {
            int s = 0;

            // Highly specific phrases (+3)
            if (text.Contains("new assignment")) s += 3;     // Google Classroom: "New assignment Case Study 2"
            if (text.Contains("assigned to you")) s += 3;
            if (text.Contains("new classwork")) s += 3;
            if (text.Contains("new material")) s += 3;
            if (text.Contains("posted on")) s += 2;          // Classroom: "Posted on 6:09 PM, May 8"
            if (text.Contains("see details")) s += 1;        // Classroom CTA
            if (text.Contains("submission deadline")) s += 3;
            if (text.Contains("submit requirement")) s += 3;
            if (text.Contains("late submission")) s += 3;
            if (text.Contains("clearance requirement")) s += 3;
            if (text.Contains("final requirement")) s += 3;
            if (text.Contains("course requirement")) s += 3;
            if (text.Contains("missing requirement")) s += 3;
            if (text.Contains("output submission")) s += 3;
            if (text.Contains("class requirement")) s += 3;
            if (text.Contains("due date")) s += 3;
            if (text.Contains("lab report")) s += 3;
            if (text.Contains("laboratory report")) s += 3;
            if (text.Contains("with due date")) s += 3;      // "assignment with due date"
            if (text.Contains("past the deadline")) s += 3;
            if (text.Contains("before the deadline")) s += 3;
            if (text.Contains("missed deadline")) s += 3;

            // Moderately specific (+2)
            if (text.Contains("assignment")) s += 2;
            if (text.Contains("project")) s += 2;
            if (text.Contains("homework")) s += 2;
            if (text.Contains("activity")) s += 2;
            if (text.Contains("output")) s += 2;
            if (text.Contains("clearance")) s += 2;
            if (text.Contains("submission")) s += 2;
            if (text.Contains("deadline")) s += 2;
            if (text.Contains("requirement")) s += 2;
            if (text.Contains("laboratory")) s += 2;
            if (text.Contains("compile")) s += 2;
            if (text.Contains("assigned")) s += 2;           // "assigned task/work"
            if (text.Contains("pass the")) s += 2;           // "pass the requirements"

            // Generic (+1)
            if (text.Contains("submit")) s += 1;
            if (text.Contains("document")) s += 1;
            if (text.Contains("due")) s += 1;

            // Context-gated: certificate and form only count alongside academic terms
            if (text.Contains("certificate") &&
                (text.Contains("clearance") || text.Contains("requirement") ||
                 text.Contains("submit") || text.Contains("academic")))
                s += 2;

            if (text.Contains("form") &&
                (text.Contains("submit") || text.Contains("requirement") ||
                 text.Contains("deadline") || text.Contains("fill out") ||
                 text.Contains("accomplish")))
                s += 2;

            return s;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────────────

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

            // Pass fromEmail so Layer 1 can check sender patterns.
            // Body is stripped of quoted content inside CategorizeEmail.
            email.Category = CategorizeEmail(
                email.Subject,
                email.Snippet + " " + email.BodyText,
                email.FromEmail);

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
            if (diff.TotalDays < 1 && dt.Date == now.Date) return dt.ToString("h:mm tt");
            if (diff.TotalDays < 7) return dt.ToString("ddd, h:mm tt");
            if (diff.TotalDays < 14) return "A week ago";
            if (diff.TotalDays < 60) return "A month ago";
            if (diff.TotalDays < 365) return dt.ToString("MMM d");
            return dt.ToString("MMM d, yyyy");
        }

        public static async Task SendReplyAsync(
            GmailService service, EmailModel originalEmail,
            string subject, string salutation, string body, string signature)
        {
            string fullBody = $"{salutation}\r\n\r\n{body}\r\n\r\n{signature}";
            string raw = BuildRawReply(
                originalEmail.FromEmail, subject, fullBody,
                originalEmail.Id, originalEmail.ThreadId);

            await service.Users.Messages.Send(
                new Google.Apis.Gmail.v1.Data.Message
                { Raw = raw, ThreadId = originalEmail.ThreadId },
                "me").ExecuteAsync();
        }

        private static string BuildRawReply(
            string to, string subject, string body,
            string inReplyToId, string threadId)
        {
            string fromEmail = AppState.UserEmail;
            string fromName = AppState.UserEmail;

            string profilePath = Path.Combine(Application.StartupPath, "profile.json");
            if (File.Exists(profilePath))
            {
                try
                {
                    var json = File.ReadAllText(profilePath);
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("FullName", out var fn) &&
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
                $"Content-Type: text/plain; charset=utf-8\r\n\r\n{body}";

            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(mime))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static async Task SendNewEmailAsync(
            GmailService service, string to, string subject, string body)
        {
            string mime =
                $"To: {to}\r\n" +
                $"From: {AppState.UserName} <{AppState.UserEmail}>\r\n" +
                $"Subject: {subject}\r\n" +
                $"Content-Type: text/plain; charset=utf-8\r\n\r\n{body}";

            await service.Users.Messages.Send(
                new Google.Apis.Gmail.v1.Data.Message
                {
                    Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(mime))
                        .Replace('+', '-').Replace('/', '_').TrimEnd('=')
                }, "me").ExecuteAsync();
        }

        private static bool Has(string text, params string[] kw)
            => kw.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}