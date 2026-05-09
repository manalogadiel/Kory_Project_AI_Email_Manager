using Google.Apis.Gmail.v1;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class SingleCompose : Form
    {
        private GmailService? _gmailService;
        private readonly HttpClient _httpClient = new HttpClient();
        private static readonly string GeminiApiKey =
    File.Exists("apikeys.txt") ? File.ReadAllText("apikeys.txt").Trim() : "";

        public SingleCompose(GmailService? gmailService = null)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            _gmailService = gmailService;

            KoryReplyBtn.Click += async (s, e) => await GenerateKoryReplyAsync();
            ImproveBtn.Click += async (s, e) => await ImproveCurrentReplyAsync();
        }

        private void SingleCompose_Load(object sender, EventArgs e) { }

        private void BackBtn_Click(object sender, EventArgs e) => this.Close();

        private string GetRecipient() => toTextBox.Text.Trim();
        private string GetSubject() => subjectTextBox.Text.Trim();

        // ── KORY REPLY ────────────────────────────────────────────────────────────
        private async Task GenerateKoryReplyAsync()
        {

            var typeDialog = new EmailTypeDialog();
            if (typeDialog.ShowDialog(this) != DialogResult.OK) return;
            string emailType = typeDialog.SelectedType;
            string extraContext = typeDialog.ExtraContext;

            string[] models = { "gemini-2.5-flash", "gemini-2.0-flash", "gemini-1.5-flash" };
            string subject = GetSubject();
            string recipient = GetRecipient();

            string prompt = $@"You are an academic email assistant named KORY helping a professor or staff write a new email.

Write a professional but natural-sounding email body for the context below.
Rules:
- Do NOT sound like AI. Write like a real person — warm, concise, human.
- Do NOT use filler phrases like 'Certainly!', 'Of course!', 'I hope this email finds you well'.
- Skip any salutation or closing signature. Write only the body.
- Keep it 2–4 short paragraphs max.
- Return ONLY a JSON object with key: ""reply"" containing the body text.

To: {recipient}
Subject: {subject}
Email Type: {emailType}
Additional Context: {extraContext}

Output format: {{""reply"": ""...""}}";

            KoryReplyBtn.Enabled = false;
            KoryReplyBtn.Text = "Generating...";

            string lastError = "";

            foreach (string model in models)
            {
                try
                {
                    string modelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={GeminiApiKey}";

                    var requestBody = new
                    {
                        contents = new[] { new { parts = new[] { new { text = prompt } } } },
                        generationConfig = new { temperature = 0.85 }
                    };

                    string jsonBody = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                    HttpResponseMessage response = await _httpClient.PostAsync(modelUrl, content, cts.Token);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        lastError = $"Model: {model}\nStatus: {response.StatusCode}\nResponse: {responseBody}";
                        continue;
                    }

                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    string generatedText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "";

                    int start = generatedText.IndexOf('{');
                    int end = generatedText.LastIndexOf('}');
                    if (start < 0 || end < 0)
                    {
                        lastError = $"Model: {model}\nNo JSON found in:\n{generatedText}";
                        continue;
                    }

                    generatedText = generatedText.Substring(start, end - start + 1);

                    using JsonDocument result = JsonDocument.Parse(generatedText);
                    string reply = result.RootElement.TryGetProperty("reply", out var r)
                        ? r.GetString() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(reply))
                    {
                        lastError = $"Model: {model}\nReply was empty.\nRaw: {generatedText}";
                        continue;
                    }

                    var confirm = MessageBox.Show(
                        $"KORY generated this:\n\n{reply.Replace("\\n", "\n")}\n\nApply to compose area?",
                        "Apply AI Reply?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                        bodyTextBox.Text = reply.Replace("\n", "\r\n");

                    KoryReplyBtn.Enabled = true;
                    KoryReplyBtn.Text = "Kory Reply";
                    return;
                }
                catch (Exception ex)
                {
                    lastError = $"Model: {model}\nException: {ex.GetType().Name}\n{ex.Message}";
                }
            }

            // Shows the REAL error now
            MessageBox.Show($"All models failed.\n\nLast error:\n{lastError}", "Debug Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            KoryReplyBtn.Enabled = true;
            KoryReplyBtn.Text = "Kory Reply";
        }

        // ── IMPROVE ───────────────────────────────────────────────────────────────
        private async Task ImproveCurrentReplyAsync()
        {
            string currentText = bodyTextBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(currentText))
            {
                MessageBox.Show("Nothing to improve. Please write something first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string[] models = { "gemini-2.5-flash", "gemini-2.0-flash" };

            string prompt = $@"You are a writing assistant. Improve the email body below.
Rules:
- Keep the exact same meaning, intent, and facts. Do NOT add new ideas.
- Make it sound more natural, clear, and professional — but still human.
- Do NOT use AI filler phrases like 'Certainly!', 'Of course!', 'I hope this finds you well'.
- Do NOT change the salutation or signature if present.
- Return ONLY a JSON object with key: ""improved"" containing the improved text.

Original:
""{currentText}""

Output format: {{""improved"": ""...""}}";

            ImproveBtn.Enabled = false;
            ImproveBtn.Text = "Improving...";

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
                    string improved = result.RootElement.TryGetProperty("improved", out var imp)
                        ? imp.GetString() ?? "" : "";

                    if (string.IsNullOrWhiteSpace(improved)) continue;

                    var confirm = MessageBox.Show(
                        $"Improved version:\n\n{improved.Replace("\\n", "\n")}\n\nApply changes?",
                        "Apply Improvements?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                        bodyTextBox.Text = improved.Replace("\n", "\r\n");

                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Improve failed with {model}: {ex.Message}");
                }
            }

            MessageBox.Show("Could not improve the text. Please try again.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            ImproveBtn.Enabled = true;
            ImproveBtn.Text = "Improve";
        }

        private void SendBtn_Click(object sender, EventArgs e) { }
        private void guna2GradientButton1_Click(object sender, EventArgs e) { }
        private void KoryReplyBtn_Click(object sender, EventArgs e) { }
        private void ImproveBtn_Click(object sender, EventArgs e) { }
    }
}