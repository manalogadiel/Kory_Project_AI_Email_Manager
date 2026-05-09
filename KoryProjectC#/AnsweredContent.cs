using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GmailMessage = Google.Apis.Gmail.v1.Data.Message;
using GmailMessagePart = Google.Apis.Gmail.v1.Data.MessagePart;
using GmailMessagePartHeader = Google.Apis.Gmail.v1.Data.MessagePartHeader;

namespace KoryProjectC_
{
    public partial class AnsweredContent : UserControl  // ← was Form
    {
        private GmailService? _service;
        private EmailModel? _sentEmail;

        public AnsweredContent()
        {
            InitializeComponent();
            guna2Button1.Click += BackBtn_Click;
        }

        private void BackBtn_Click(object? sender, EventArgs e)
        {
            var home = Application.OpenForms.OfType<Home>().FirstOrDefault();
            if (home == null) return;
            home.ShowAnswered();
        }

        public async Task LoadAsync(EmailModel sentEmail, GmailService service)
        {
            _sentEmail = sentEmail;
            _service = service;

            guna2HtmlLabel2.Text = !string.IsNullOrWhiteSpace(sentEmail.Subject)
                ? sentEmail.Subject : "(No Subject)";
            guna2HtmlLabel4.Text = sentEmail.FromName;
            guna2HtmlLabel3.Text = sentEmail.FromEmail;
            guna2Button3.Text = sentEmail.Date;

            string sentHtml = !string.IsNullOrEmpty(sentEmail.BodyHtml)
                ? InjectDarkModeStyles(sentEmail.BodyHtml, "#0f1020")
                : BuildPlainHtml(sentEmail.BodyText ?? sentEmail.Snippet ?? "", "#0f1020");

            await InitWebViewAsync(sentEmailContent, sentHtml, Color.FromArgb(15, 16, 32));

            try
            {
                var threadReq = service.Users.Threads.Get("me", sentEmail.ThreadId);
                threadReq.Format = UsersResource.ThreadsResource.GetRequest.FormatEnum.Full;
                var thread = await threadReq.ExecuteAsync();

                var original = thread.Messages?
                    .FirstOrDefault(m => m.Id != sentEmail.Id);

                if (original != null)
                {
                    var orig = ParseMessage(original);

                    guna2HtmlLabel1.Text = !string.IsNullOrWhiteSpace(orig.Subject)
                        ? orig.Subject : "(No Subject)";
                    NameForm.Text = orig.FromName;
                    EmailForm.Text = orig.FromEmail;
                    guna2Button2.Text = orig.Date;

                    string origHtml = !string.IsNullOrEmpty(orig.BodyHtml)
                        ? InjectDarkModeStyles(orig.BodyHtml, "#0e0f14")
                        : BuildPlainHtml(orig.BodyText ?? orig.Snippet ?? "", "#0e0f14");

                    await InitWebViewAsync(EmailContent, origHtml, Color.FromArgb(14, 15, 20));
                }
                else
                {
                    guna2HtmlLabel1.Text = "(No original email found)";
                    NameForm.Text = "";
                    EmailForm.Text = "";
                    guna2Button2.Text = "";

                    await InitWebViewAsync(EmailContent,
                        BuildPlainHtml("No original email found in this thread.", "#0e0f14"),
                        Color.FromArgb(14, 15, 20));
                }
            }
            catch (Exception ex)
            {
                guna2HtmlLabel1.Text = "(Could not load original)";
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static async Task InitWebViewAsync(WebView2 wv, string html, Color bgColor)
        {
            try
            {
                // Wait until the control has a valid window handle
                int attempts = 0;
                while (!wv.IsHandleCreated && attempts < 20)
                {
                    await Task.Delay(100);
                    attempts++;
                }

                if (wv.IsDisposed) return;

                await wv.EnsureCoreWebView2Async(null);

                wv.DefaultBackgroundColor = bgColor;
                wv.NavigateToString(html);
            }
            catch (Exception ex) when (ex is System.Runtime.InteropServices.COMException
                                     || ex is System.InvalidOperationException)
            {
                await Task.Delay(500);
                try
                {
                    if (wv.IsDisposed) return;
                    await wv.EnsureCoreWebView2Async(null);
                    wv.DefaultBackgroundColor = bgColor;
                    wv.NavigateToString(html);
                }
                catch { }
            }
        }

        private static string InjectDarkModeStyles(string html, string bgHex)
        {
            string style = $@"
<style>
  html, body, div, p, span, td, th, li, a, h1, h2, h3, h4, h5, h6,
  table, tr, section, article, header, footer, main, blockquote {{
      background-color: {bgHex} !important;
      color: #d4d4d4 !important;
      font-family: Segoe UI, sans-serif !important;
      font-size: 14px !important;
  }}
  a {{ color: #7b9fff !important; }}
  img {{ opacity: 0.9; }}
  [style*='background:#fff'],
  [style*='background: #fff'],
  [style*='background:white'],
  [style*='background-color:#fff'],
  [style*='background-color: #fff'],
  [style*='background-color:white'] {{
      background-color: {bgHex} !important;
  }}
  [style*='color:#000'],
  [style*='color: #000'],
  [style*='color:black'],
  [style*='color:#333'],
  [style*='color:#222'],
  [style*='color:#111'] {{
      color: #d4d4d4 !important;
  }}
</style>";

            if (html.Contains("</head>", StringComparison.OrdinalIgnoreCase))
                return html.Replace("</head>", style + "</head>",
                    StringComparison.OrdinalIgnoreCase);
            if (html.Contains("<body", StringComparison.OrdinalIgnoreCase))
                return html.Replace("<body", style + "<body",
                    StringComparison.OrdinalIgnoreCase);
            return style + html;
        }

        private static string BuildPlainHtml(string text, string bgHex)
        {
            var encoded = System.Net.WebUtility.HtmlEncode(text)
                .Replace("&#xA;", "<br>")
                .Replace("\n", "<br>");

            return $@"<html>
<head><meta charset='utf-8'></head>
<body style='
    color: #d4d4d4;
    background-color: {bgHex};
    font-family: Segoe UI, sans-serif;
    font-size: 14px;
    padding: 24px;
    line-height: 1.7;
    word-wrap: break-word;'>
{encoded}
</body></html>";
        }

        private static EmailModel ParseMessage(GmailMessage message)
        {
            var email = new EmailModel
            {
                Id = message.Id ?? "",
                ThreadId = message.ThreadId ?? "",
                Snippet = message.Snippet ?? "",
                IsRead = !(message.LabelIds?.Contains("UNREAD") ?? false)
            };

            foreach (var h in message.Payload?.Headers
                ?? Enumerable.Empty<GmailMessagePartHeader>())
            {
                switch (h.Name?.ToLower())
                {
                    case "subject":
                        email.Subject = h.Value ?? "(No Subject)";
                        break;
                    case "from":
                        var match = Regex.Match(
                            h.Value ?? "", @"^""?(.+?)""?\s*<(.+?)>$");
                        if (match.Success)
                        {
                            email.FromName = match.Groups[1].Value.Trim();
                            email.FromEmail = match.Groups[2].Value.Trim();
                        }
                        else
                        {
                            email.FromName = h.Value ?? "";
                            email.FromEmail = h.Value ?? "";
                        }
                        break;
                    case "date":
                        email.Date = FormatDate(h.Value);
                        break;
                }
            }

            email.BodyHtml = GetBody(message.Payload, "text/html");
            email.BodyText = GetBody(message.Payload, "text/plain");

            if (string.IsNullOrWhiteSpace(email.Subject))
                email.Subject = "(No Subject)";

            return email;
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

        private static string GetBody(GmailMessagePart? part, string mimeType)
        {
            if (part == null) return "";
            if (part.MimeType == mimeType && part.Body?.Data != null)
            {
                var b64 = part.Body.Data.Replace('-', '+').Replace('_', '/');
                b64 = b64.PadRight(b64.Length + (4 - b64.Length % 4) % 4, '=');
                return System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(b64));
            }
            if (part.Parts != null)
                foreach (var sub in part.Parts)
                {
                    var result = GetBody(sub, mimeType);
                    if (!string.IsNullOrEmpty(result)) return result;
                }
            return "";
        }

        private void AnsweredContent_Load(object sender, EventArgs e) { }
        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }
    }
}