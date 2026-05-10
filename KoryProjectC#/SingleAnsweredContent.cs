using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GmailMessage = Google.Apis.Gmail.v1.Data.Message;
using GmailMessagePart = Google.Apis.Gmail.v1.Data.MessagePart;
using GmailMessagePartHeader = Google.Apis.Gmail.v1.Data.MessagePartHeader;

namespace KoryProjectC_
{
    public partial class SingleAnsweredContent : Form
    {
        private WebView2? _webView;

        public SingleAnsweredContent()
        {
            InitializeComponent();


            bgPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                             AnchorStyles.Left | AnchorStyles.Right;

            _webView = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = Color.FromArgb(15, 16, 32),
            };
            bgPanel.Controls.Add(_webView);

            guna2Button1.Click += BackBtn_Click;
        }

        // ── LOAD FROM EMAILMODEL ─────────────────────────────────────────────

        public async Task LoadFromEmail(EmailModel email, GmailService service)
        {
            guna2HtmlLabel4.Text = email.FromName ?? "";
            guna2HtmlLabel3.Text = email.FromEmail ?? "";
            guna2HtmlLabel2.Text = !string.IsNullOrWhiteSpace(email.Subject)
                ? email.Subject : "(No Subject)";
            guna2Button3.Text = email.Date ?? "";

            SetAvatar(guna2CirclePictureBox2, email.FromName ?? "", email.FromEmail ?? "");

            string html = !string.IsNullOrEmpty(email.BodyHtml)
                ? InjectDarkModeStyles(email.BodyHtml)
                : BuildPlainHtml(email.BodyText ?? email.Snippet ?? "");

            try
            {
                await _webView!.EnsureCoreWebView2Async(null);
                _webView.NavigateToString(html);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 error: {ex.Message}");
            }
        }

        // ── LOAD FROM DRAFTMODEL ─────────────────────────────────────────────

        public async Task LoadFromDraft(DraftModel draft)
        {
            guna2HtmlLabel4.Text = draft.Original?.FromName
                                   ?? draft.Original?.FromEmail
                                   ?? "";
            guna2HtmlLabel3.Text = draft.Original?.FromEmail ?? "";
            guna2HtmlLabel2.Text = !string.IsNullOrWhiteSpace(draft.Subject)
                ? draft.Subject : "(No Subject)";
            guna2Button3.Text = DateTime.Now.ToString("ddd, h:mm tt");

            SetAvatar(guna2CirclePictureBox2,
                draft.Original?.FromName ?? "",
                draft.Original?.FromEmail ?? "");

            string html = BuildPlainHtml(draft.Body ?? "");

            try
            {
                await _webView!.EnsureCoreWebView2Async(null);
                _webView.NavigateToString(html);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 error: {ex.Message}");
            }
        }

        // ── BACK BUTTON ──────────────────────────────────────────────────────

        private void BackBtn_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        // ── AVATAR ───────────────────────────────────────────────────────────

        private static void SetAvatar(
            Guna.UI2.WinForms.Guna2CirclePictureBox box,
            string fromName, string fromEmail)
        {
            try
            {
                string initials = GetInitials(fromName, fromEmail);
                Color avatarColor = GetAvatarColor(fromName + fromEmail);

                int size = box.Width;
                var bmp = new Bitmap(size, size);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode =
                        System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint =
                        System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.FillEllipse(new SolidBrush(avatarColor), 0, 0, size, size);

                    float fontSize = size * 0.35f;
                    var font = new Font("Segoe UI", fontSize,
                        FontStyle.Bold, GraphicsUnit.Pixel);
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(initials, font, new SolidBrush(Color.White),
                        new RectangleF(0, 0, size, size), format);
                }

                box.Image = bmp;
            }
            catch { }
        }

        private static string GetInitials(string name, string email)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Trim().Split(' ',
                    StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
                if (parts.Length == 1 && parts[0].Length > 0)
                    return parts[0][0].ToString().ToUpper();
            }
            if (!string.IsNullOrWhiteSpace(email))
                return email[0].ToString().ToUpper();
            return "?";
        }

        private static Color GetAvatarColor(string seed)
        {
            Color[] palette =
            {
                Color.FromArgb(98,  117, 217),
                Color.FromArgb(76,  175, 130),
                Color.FromArgb(229, 115,  95),
                Color.FromArgb(100, 160, 220),
                Color.FromArgb(186, 104, 200),
                Color.FromArgb(77,  182, 172),
                Color.FromArgb(240, 154,  56),
                Color.FromArgb(129, 199, 132),
                Color.FromArgb(229, 115, 155),
                Color.FromArgb(111, 143, 175),
            };
            int hash = 0;
            foreach (char c in seed)
                hash = (hash * 31 + c) & 0x7fffffff;
            return palette[hash % palette.Length];
        }

        // ── HTML HELPERS ─────────────────────────────────────────────────────

        private static string InjectDarkModeStyles(string html)
        {
            const string style = @"
<style>
  html, body, div, p, span, td, th, li, a, h1, h2, h3, h4, h5, h6,
  table, tr, section, article, header, footer, main, blockquote {
      background-color: #0f1020 !important;
      color: #d4d4d4 !important;
      font-family: Segoe UI, sans-serif !important;
      font-size: 14px !important;
  }
  a { color: #7b9fff !important; }
  img { opacity: 0.9; }
  [style*='background:#fff'],
  [style*='background-color:#fff'],
  [style*='background-color:white'] { background-color: #0f1020 !important; }
  [style*='color:#000'],
  [style*='color:black'],
  [style*='color:#333'] { color: #d4d4d4 !important; }
</style>";

            if (html.Contains("</head>", StringComparison.OrdinalIgnoreCase))
                return html.Replace("</head>", style + "</head>",
                    StringComparison.OrdinalIgnoreCase);
            if (html.Contains("<body", StringComparison.OrdinalIgnoreCase))
                return html.Replace("<body", style + "<body",
                    StringComparison.OrdinalIgnoreCase);
            return style + html;
        }

        private static string BuildPlainHtml(string text)
        {
            var encoded = System.Net.WebUtility.HtmlEncode(text)
                .Replace("&#xA;", "<br>")
                .Replace("\n", "<br>");

            return $@"<html>
<head><meta charset='utf-8'></head>
<body style='
    color:#d4d4d4;
    background-color:#0f1020;
    font-family:Segoe UI,sans-serif;
    font-size:14px;
    padding:24px;
    line-height:1.7;
    word-wrap:break-word;'>
{encoded}
</body></html>";
        }
    }
}