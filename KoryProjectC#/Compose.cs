using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace KoryProjectC_

{
    public partial class Compose : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private static readonly string GeminiApiKey =
    File.Exists("apikeys.txt") ? File.ReadAllText("apikeys.txt").Trim() : "";  
        

        public Compose()
        {
            InitializeComponent();
            btnAnalyze.Click += async (s, e) => await AnalyzeText();
            
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            

            guna2Button1.Click += (_, _) =>
            {
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
            bar.Text = $"{value}%";

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

        public async void LoadEmail(EmailModel email)
        {
            NameForm.Text = email.FromName ?? "";
            EmailForm.Text = email.FromEmail ?? "";
            guna2HtmlLabel1.Text = email.Subject ?? "(no subject)";
            guna2Button2.Text = email.Date ?? "";

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
        }

        /// <summary>
        /// Injects CSS into an HTML email to force light text on a dark background.
        /// </summary>
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



    }
}










        
        

