using Google.Apis.Gmail.v1;
using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KoryProjectC_

{
    public partial class Compose : Form
    {

        private EmailModel? _currentEmail;
        private GmailService? _gmailService;
        private readonly HttpClient _httpClient = new HttpClient();
        private static readonly string GeminiApiKey =
        File.Exists("apikeys.txt") ? File.ReadAllText("apikeys.txt").Trim() : "AIzaSyACJA_l8JJ9gXJsva7nBTiNH6x1rEFAdhI";


        public Compose()
        {
            InitializeComponent();
            KoryReplyBtn.Click += async (s, e) => await GenerateKoryReplyAsync();
            ImproveBtn.Click += async (s, e) => await ImproveCurrentReplyAsync();
            btnAnalyze.Click += async (s, e) => await AnalyzeText();
            Guna2TextBox1.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(Guna2TextBox1.Text))
                {
                    txtInput.Text = Guna2TextBox1.Text;
                }
            };

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;

            // Hook the Edit Profile button
            guna2GradientButton1.Click -= guna2GradientButton1_Click; // remove old empty handler
            guna2GradientButton1.Click += (s, e) =>
            {
                var profileForm = new ProfileForm();
                profileForm.LoadSavedProfile();  // load saved data

                var composeScreen = this.RectangleToScreen(this.ClientRectangle);
                profileForm.Location = new Point(
                    composeScreen.Left + (composeScreen.Width - profileForm.Width) / 2,
                    composeScreen.Top + (composeScreen.Height - profileForm.Height) / 2
                );

                profileForm.OnSaved += (sender, args) =>
                {
                    Guna2TextBox1.Text = profileForm.txtPreview.Text
                        .Replace("\r\n\r\n", " • ")
                        .Replace("\r\n", " • ");
                };

                profileForm.ShowDialog(this.FindForm());
            };

            guna2Button1.Click += (_, _) =>
            {
                Application.OpenForms
                    .OfType<Home>()
                    .FirstOrDefault()
                    ?.HideFullscreenCompose();
            };
            SendBtn.Click += async (s, e) =>
            {
                if (_currentEmail == null || _gmailService == null) return;

                if (string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    MessageBox.Show("Please write a reply first.", "Empty",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SendBtn.Enabled = false;
                SendBtn.Text = "Sending...";

                try
                {
                    string signature = Guna2TextBox1.Text
                       .Replace(" • ", "\r\n")
                       .Replace("•", "\r\n");

                    await GmailHelper.SendReplyAsync(
                        _gmailService,
                        _currentEmail,
                        txtSubject.Text,
                        txtSalutation.Text,
                        txtInput.Text,
                        signature
                    );

                    MessageBox.Show("Reply sent successfully!", "Sent",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DraftHelper.DeleteDraft(_currentEmail.Id);

                    Application.OpenForms
                        .OfType<Home>()
                        .FirstOrDefault()
                        ?.HideFullscreenCompose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to send: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SendBtn.Enabled = true;
                    SendBtn.Text = "Send";
                }

            };
            SaveDraftBtn.Click += (s, e) =>
            {
                if (_currentEmail == null) return;

                var draft = new DraftModel
                {
                    EmailId = _currentEmail.Id,
                    Subject = txtSubject.Text,
                    Salutation = txtSalutation.Text,
                    Body = txtInput.Text,
                    Signature = Guna2TextBox1.Text,
                    Original = _currentEmail
                };

                DraftHelper.SaveDraft(draft);

                MessageBox.Show("Draft saved!", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Go back to inbox
                Application.OpenForms
                    .OfType<Home>()
                    .FirstOrDefault()
                    ?.HideFullscreenCompose();
            };
        }

        private async Task AnalyzeText()
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("Please enter some text to analyze.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnAnalyze.Enabled = false;
            btnAnalyze.Text = "Analyzing...";

            // Define models to try (primary first, then fallback)
            string[] models = { "gemini-2.5-flash", "gemini-2.0-flash" };
            bool success = false;

            foreach (string model in models)
            {
                success = await TryAnalyzeWithModel(model);
                if (success) break;
            }

            if (!success)
            {
                MessageBox.Show("All models failed. Please try again later.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnAnalyze.Enabled = true;
            btnAnalyze.Text = "Analyze";
        }

        private async Task<bool> TryAnalyzeWithModel(string modelName)
        {
            string modelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key=" + GeminiApiKey;
            int timeoutSeconds = 60; // 60 seconds timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                var prompt = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = $@"You are a text analysis assistant. Analyze the following text and return ONLY a JSON object with keys: clarity (0-100), tone (0-100), prof (0-100). Do not include any explanation or extra text.

Text: ""{txtInput.Text}""

Output format: {{""clarity"": number, ""tone"": number, ""prof"": number}}"
                        }
                    }
                }
            },
                    generationConfig = new { temperature = 0.2 }
                };

                string jsonBody = JsonSerializer.Serialize(prompt);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(modelUrl, content, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    string generatedText = doc.RootElement.GetProperty("candidates")[0]
                        .GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                    int start = generatedText.IndexOf('{');
                    int end = generatedText.LastIndexOf('}');
                    if (start >= 0 && end >= 0)
                        generatedText = generatedText.Substring(start, end - start + 1);

                    var scores = JsonSerializer.Deserialize<ScoreResponse>(generatedText);
                    if (scores != null)
                    {
                        UpdateProgressBar(progressClarity, scores.clarity);
                        UpdateProgressBar(progressTone, scores.tone);
                        UpdateProgressBar(progressProf, scores.prof);
                        return true;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // Rate limit – maybe retry after a delay, but for simplicity we just fall back
                    await Task.Delay(2000);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    // Model overload – fall back to next model
                    await Task.Delay(1000);
                }
                else
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error with {modelName}: {response.StatusCode}\n{errorBody}",
                        "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout – fall back to next model
                MessageBox.Show($"Model {modelName} timed out after {timeoutSeconds} seconds. Trying next model...",
                    "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception with {modelName}: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
        private void UpdateProgressBar(Guna2CircleProgressBar bar, int value)
        {
            if (bar.InvokeRequired)
            {
                bar.Invoke(new Action(() => UpdateProgressBar(bar, value)));
                return;
            }

            bar.Value = value;

            // Show percentage inside the circle (e.g., "85%")
            bar.Text = $"{value}";

            // Change color based on value
            if (value > 80)
                bar.ProgressColor = Color.LimeGreen;
            else if (value >= 50)
                bar.ProgressColor = Color.Goldenrod;
            else
                bar.ProgressColor = Color.Crimson;
        }

        // Helper class to deserialize the JSON from Gemini
        private class ScoreResponse
        {
            public int clarity { get; set; }
            public int tone { get; set; }
            public int prof { get; set; }
        }


        private void SubjectForm_TextChanged(object sender, EventArgs e)
        {

        }

        private void EmailForm_Click(object sender, EventArgs e)
        {

        }

        private void NameForm_Click(object sender, EventArgs e)
        {

        }

        private void Guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Compose_Load(object sender, EventArgs e)
        {
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click_1(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {

        }

        private void EmailContent_Click(object sender, EventArgs e)
        {

        }

        private void ComposingWriteForm_TextChanged(object sender, EventArgs e)
        {

        }

        private void Guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public async void LoadEmail(EmailModel email, GmailService service)
        {
            _currentEmail = email;
            _gmailService = service;

            NameForm.Text = email.FromName ?? "";
            EmailForm.Text = email.FromEmail ?? "";
            guna2HtmlLabel1.Text = email.Subject ?? "(no subject)";
            guna2Button2.Text = email.Date ?? "";

            string firstName = (email.FromName ?? "").Split(' ')[0];
            txtSalutation.Text = $"Dear {firstName},";

            string html = !string.IsNullOrEmpty(email.BodyHtml)
                ? InjectDarkModeStyles(email.BodyHtml)
                : BuildPlainHtml(email.BodyText ?? "");

            try
            {
                await EmailContent.EnsureCoreWebView2Async(null);
                EmailContent.NavigateToString(html);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 failed: {ex.Message}");
                EmailContent.NavigateToString(BuildPlainHtml(email.BodyText ?? ""));
            }

            await GenerateAiFieldsAsync(email);

            // Auto-load saved profile into signature box
            string profilePath = Path.Combine(Application.StartupPath, "profile.json");
            if (File.Exists(profilePath))
            {
                try
                {
                    var json = File.ReadAllText(profilePath);
                    var data = System.Text.Json.JsonSerializer.Deserialize<ProfileForm.ProfileData>(json);
                    if (data != null)
                    {
                        string preview =
                            $"{data.ComplementaryClose}\r\n\r\n" +
                            $"{data.FullName}\r\n" +
                            $"{data.Title}\r\n" +
                            $"{data.Department}";

                        Guna2TextBox1.Text = preview
                            .Replace("\r\n\r\n", " • ")
                            .Replace("\r\n", " • ");
                    }
                }
                catch { }
            }
        }
        private static string InjectDarkModeStyles(string html)
        {
            const string style = @"
<style>
  html, body, div, p, span, td, th, li, a, h1, h2, h3, h4, h5, h6,
  table, tr, section, article, header, footer, main, blockquote {
      background-color: #0e0f14 !important;
      color: #d4d4d4 !important;
  }
  a {
      color: #7b9fff !important;
  }
  img {
      opacity: 0.9;
  }
  /* Kill white/light backgrounds that Google emails love to add */
  [style*='background:#fff'],
  [style*='background: #fff'],
  [style*='background:white'],
  [style*='background-color:#fff'],
  [style*='background-color: #fff'],
  [style*='background-color:white'] {
      background-color: #0e0f14 !important;
  }
  [style*='color:#000'],
  [style*='color: #000'],
  [style*='color:black'],
  [style*='color:#333'],
  [style*='color:#222'],
  [style*='color:#111'] {
      color: #d4d4d4 !important;
  }
</style>";

            // Inject right before </head> if it exists, otherwise prepend
            if (html.Contains("</head>", StringComparison.OrdinalIgnoreCase))
                return html.Replace("</head>", style + "</head>",
                    StringComparison.OrdinalIgnoreCase);

            if (html.Contains("<body", StringComparison.OrdinalIgnoreCase))
                return html.Replace("<body", style + "<body",
                    StringComparison.OrdinalIgnoreCase);

            // Bare HTML with no head/body tags
            return style + html;
        }

        /// <summary>Plain text fallback wrapper.</summary>
        private static string BuildPlainHtml(string text)
        {
            var encoded = System.Net.WebUtility.HtmlEncode(text)
                              .Replace("&#xA;", "<br>")
                              .Replace("\n", "<br>");

            return $@"<html>
<head><meta charset='utf-8'></head>
<body style='
    color: #d4d4d4;
    background-color: #0e0f14;
    font-family: Segoe UI, sans-serif;
    font-size: 14px;
    padding: 24px;
    line-height: 1.7;
    word-wrap: break-word;'>
{encoded}
</body></html>";
        }

        private async Task GenerateAiFieldsAsync(EmailModel email)
        {
            string[] models = { "gemini-2.5-flash", "gemini-2.0-flash" };

            string emailContext = $@"
Sender: {email.FromName} <{email.FromEmail}>
Subject: {email.Subject}
Body:
{(string.IsNullOrEmpty(email.BodyText) ? "(HTML email — no plain text)" : email.BodyText)}
".Trim();

            if (this.IsHandleCreated && !this.IsDisposed)
                this.Invoke(() => txtSubject.PlaceholderText = "Generating subject...");

            string prompt = $@"You are an academic email assistant named KORY. Given the email below, generate only a reply subject line.

Return ONLY a JSON object with exactly this key:
- ""subject"": a short reply subject line (e.g. ""Re: Grade Inquiry for CS1101"")

Email:
{emailContext}

Output format: {{""subject"": ""...""}}";

            foreach (string model in models)
            {
                try
                {
                    string modelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={GeminiApiKey}";

                    var requestBody = new
                    {
                        contents = new[]
                        {
                    new { parts = new[] { new { text = prompt } } }
                },
                        generationConfig = new { temperature = 0.4 }
                    };

                    string jsonBody = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    HttpResponseMessage response = await _httpClient.PostAsync(modelUrl, content, cts.Token);

                    if (!response.IsSuccessStatusCode) continue;

                    string responseBody = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    string generatedText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "";

                    int start = generatedText.IndexOf('{');
                    int end = generatedText.LastIndexOf('}');
                    if (start < 0 || end < 0) continue;
                    generatedText = generatedText.Substring(start, end - start + 1);

                    using JsonDocument result = JsonDocument.Parse(generatedText);
                    string subject = result.RootElement.TryGetProperty("subject", out var s)
                        ? s.GetString() ?? "" : "";

                    if (this.IsHandleCreated && !this.IsDisposed)
                        this.Invoke(() =>
                        {
                            txtSubject.Text = subject;
                            txtSubject.PlaceholderText = "";
                        });

                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"AI generation failed with {model}: {ex.Message}");
                }
            }

            if (this.IsHandleCreated && !this.IsDisposed)
                this.Invoke(() => txtSubject.PlaceholderText = "Could not generate subject");
        }

        // ── KORY REPLY ──────────────────────────────────────────────────────────────
        private async Task GenerateKoryReplyAsync()
        {
            string[] models = { "gemini-2.5-flash", "gemini-2.0-flash" };

            // Grab current email context from the loaded fields
            string emailContext = $@"
Sender: {NameForm.Text} <{EmailForm.Text}>
Subject: {guna2HtmlLabel1.Text}
".Trim();

            string salutation = txtSalutation.Text;

            string prompt = $@"You are an academic email assistant named KORY helping a professor or staff reply to a student email.

Write a professional but natural-sounding reply to the email below.
Rules:
- Do NOT sound like AI. Write like a real person — warm, concise, human.
- Do NOT use filler phrases like 'Certainly!', 'Of course!', 'I hope this email finds you well', 'Please do not hesitate'.
- Skip the salutation line (it is already handled separately as: ""{salutation}"").
- Skip any closing signature line (e.g. 'Sincerely, ...'). End naturally.
- Keep it 2–4 short paragraphs max.
- Return ONLY a JSON object with key: ""reply"" containing the reply body text.

Email to reply to:
{emailContext}

Output format: {{""reply"": ""...""}}";

            foreach (string model in models)
            {
                try
                {
                    string modelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={GeminiApiKey}";

                    var requestBody = new
                    {
                        contents = new[] { new { parts = new[] { new { text = prompt } } } },
                        generationConfig = new { temperature = 0.85 }  // higher temp = more human-sounding
                    };

                    string jsonBody = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    HttpResponseMessage response = await _httpClient.PostAsync(modelUrl, content, cts.Token);

                    if (!response.IsSuccessStatusCode) continue;

                    string responseBody = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    string generatedText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "";

                    int start = generatedText.IndexOf('{');
                    int end = generatedText.LastIndexOf('}');
                    if (start < 0 || end < 0) continue;
                    generatedText = generatedText.Substring(start, end - start + 1);

                    using JsonDocument result = JsonDocument.Parse(generatedText);
                    string reply = result.RootElement.TryGetProperty("reply", out var r) ? r.GetString() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(reply)) continue;

                    // Ask user to apply
                    var confirm = MessageBox.Show(
    $"KORY generated this reply:\n\n{reply.Replace("\\n", "\n")}\n\nApply to compose area?",
    "Apply AI Reply?",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        this.Invoke(() => txtInput.Text = reply.Replace("\n", "\r\n"));
                    }

                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"KORY Reply failed with {model}: {ex.Message}");
                }
            }

            MessageBox.Show("Could not generate a reply. Please try again.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ── IMPROVE ──────────────────────────────────────────────────────────────────
        private async Task ImproveCurrentReplyAsync()
        {
            string currentText = txtInput.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(currentText))
            {
                MessageBox.Show("Nothing to improve. Please write something first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string[] models = { "gemini-2.5-flash", "gemini-2.0-flash" };

            string prompt = $@"You are a writing assistant. Improve the email reply below.
Rules:
- Keep the exact same meaning, intent, and facts. Do NOT add new ideas.
- Make it sound more natural, clear, and professional — but still human.
- Do NOT use AI filler phrases like 'Certainly!', 'Of course!', 'I hope this finds you well'.
- Do NOT change the salutation or signature if present.
- Return ONLY a JSON object with key: ""improved"" containing the improved text.

Original reply:
""{currentText}""

Output format: {{""improved"": ""...""}}";

            foreach (string model in models)
            {
                try
                {
                    string modelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={GeminiApiKey}";

                    var requestBody = new
                    {
                        contents = new[] { new { parts = new[] { new { text = prompt } } } },
                        generationConfig = new { temperature = 0.5 }
                    };

                    string jsonBody = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    HttpResponseMessage response = await _httpClient.PostAsync(modelUrl, content, cts.Token);

                    if (!response.IsSuccessStatusCode) continue;

                    string responseBody = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    string generatedText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "";

                    int start = generatedText.IndexOf('{');
                    int end = generatedText.LastIndexOf('}');
                    if (start < 0 || end < 0) continue;
                    generatedText = generatedText.Substring(start, end - start + 1);

                    using JsonDocument result = JsonDocument.Parse(generatedText);
                    string improved = result.RootElement.TryGetProperty("improved", out var imp) ? imp.GetString() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(improved)) continue;

                    var confirm = MessageBox.Show(
    $"Improved version:\n\n{improved.Replace("\\n", "\n")}\n\nApply changes?",
    "Apply Improvements?",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        this.Invoke(() => txtInput.Text = improved.Replace("\n", "\r\n"));
                    }

                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Improve failed with {model}: {ex.Message}");
                }
            }

            MessageBox.Show("Could not improve the text. Please try again.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void LoadDraft(DraftModel draft)
        {
            txtSubject.Text = draft.Subject;
            txtSalutation.Text = draft.Salutation;
            txtInput.Text = draft.Body;
            Guna2TextBox1.Text = draft.Signature;
        }


    }

}