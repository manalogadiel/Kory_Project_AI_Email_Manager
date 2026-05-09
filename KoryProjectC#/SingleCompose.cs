using Google.Apis.Gmail.v1;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace KoryProjectC_
{
    public partial class SingleCompose : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private static readonly string GeminiApiKey =
            File.Exists("apikeys.txt") ? File.ReadAllText("apikeys.txt").Trim() : "";

        public SingleCompose()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;

            // Auto-load saved profile into Guna2TextBox1
            LoadSavedProfile();

            // Analyze
            btnAnalyze.Click += async (s, e) => await AnalyzeTextAsync();

            // Edit Profile
            EditBtn.Click += (s, e) =>
            {
                var profileForm = new ProfileForm();
                profileForm.LoadSavedProfile();

                var screen = this.RectangleToScreen(this.ClientRectangle);
                profileForm.Location = new Point(
                    screen.Left + (screen.Width - profileForm.Width) / 2,
                    screen.Top + (screen.Height - profileForm.Height) / 2
                );

                profileForm.OnSaved += (sender, args) =>
                {
                    Guna2TextBox1.Text = profileForm.txtPreview.Text
                        .Replace("\r\n\r\n", " • ")
                        .Replace("\r\n", " • ");
                };

                profileForm.ShowDialog(this);
            };

            // Save Draft
            SaveDraftBtn.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(guna2TextBox3.Text))
                {
                    MessageBox.Show("Please enter a recipient.", "Empty",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var draft = new DraftModel
                {
                    EmailId = $"single_{DateTime.Now.Ticks}",
                    Subject = subjectTextBox.Text,
                    Salutation = Guna2TextBox2.Text,
                    Body = txtInput.Text,
                    Signature = Guna2TextBox1.Text,
                    Original = new EmailModel
                    {
                        FromEmail = guna2TextBox3.Text,
                        Subject = subjectTextBox.Text
                    }
                };

                DraftHelper.SaveDraft(draft);
                MessageBox.Show("Draft saved!", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            };

            // Send
            SendBtn.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(guna2TextBox3.Text))
                {
                    MessageBox.Show("Please enter a recipient.", "Empty",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    MessageBox.Show("Please write a message.", "Empty",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (AppState.GmailService == null)
                {
                    MessageBox.Show("Gmail service not available.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SendBtn.Enabled = false;
                SendBtn.Text = "Sending...";

                try
                {
                    string signature = Guna2TextBox1.Text
                        .Replace(" • ", "\r\n")
                        .Replace("•", "\r\n");

                    string fullBody =
                        $"{Guna2TextBox2.Text}\r\n\r\n" +
                        $"{txtInput.Text}\r\n\r\n" +
                        $"{signature}";

                    await GmailHelper.SendNewEmailAsync(
                        AppState.GmailService,
                        guna2TextBox3.Text,
                        subjectTextBox.Text,
                        fullBody
                    );

                    MessageBox.Show("Email sent successfully!", "Sent",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
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
        }

        private void LoadSavedProfile()
        {
            string profilePath = Path.Combine(Application.StartupPath, "profile.json");
            if (!File.Exists(profilePath)) return;
            try
            {
                var json = File.ReadAllText(profilePath);
                var data = JsonSerializer.Deserialize<ProfileForm.ProfileData>(json);
                if (data == null) return;

                string preview =
                    $"{data.ComplementaryClose}\r\n\r\n" +
                    $"{data.FullName}\r\n" +
                    $"{data.Title}\r\n" +
                    $"{data.Department}";

                Guna2TextBox1.Text = preview
                    .Replace("\r\n\r\n", " • ")
                    .Replace("\r\n", " • ");
            }
            catch { }
        }

        private async Task AnalyzeTextAsync()
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("Please enter some text to analyze.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnAnalyze.Enabled = false;
            btnAnalyze.Text = "Analyzing...";

            string[] models = { "gemini-2.5-flash", "gemini-2.0-flash" };
            bool success = false;

            foreach (string model in models)
            {
                success = await TryAnalyzeWithModel(model);
                if (success) break;
            }

            if (!success)
                MessageBox.Show("All models failed. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            btnAnalyze.Enabled = true;
            btnAnalyze.Text = "Analyze";
        }

        private async Task<bool> TryAnalyzeWithModel(string modelName)
        {
            string modelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={GeminiApiKey}";

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
                                    text = $@"Analyze the following text and return ONLY a JSON object with keys: clarity (0-100), tone (0-100), prof (0-100).

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

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                var response = await _httpClient.PostAsync(modelUrl, content, cts.Token);

                if (!response.IsSuccessStatusCode) return false;

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
                if (start < 0 || end < 0) return false;
                generatedText = generatedText.Substring(start, end - start + 1);

                var scores = JsonSerializer.Deserialize<ScoreResponse>(generatedText);
                if (scores == null) return false;

                UpdateProgressBar(progressClarity, scores.clarity);
                UpdateProgressBar(progressTone, scores.tone);
                UpdateProgressBar(progressProf, scores.prof);
                return true;
            }
            catch { return false; }
        }

        private void UpdateProgressBar(Guna2CircleProgressBar bar, int value)
        {
            if (bar.InvokeRequired) { bar.Invoke(() => UpdateProgressBar(bar, value)); return; }
            bar.Value = value;
            bar.Text = $"{value}%";
            bar.ProgressColor = value > 80 ? Color.LimeGreen : value >= 50 ? Color.Goldenrod : Color.Crimson;
        }

        private class ScoreResponse
        {
            public int clarity { get; set; }
            public int tone { get; set; }
            public int prof { get; set; }
        }

        private void SingleCompose_Load(object sender, EventArgs e) { }
        private void BackBtn_Click(object sender, EventArgs e) => this.Close();
        private void guna2GradientButton1_Click(object sender, EventArgs e) { }
        private void SendBtn_Click(object sender, EventArgs e) { }
    }
}