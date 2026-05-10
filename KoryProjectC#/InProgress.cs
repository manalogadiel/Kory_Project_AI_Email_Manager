using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class InProgress : UserControl
    {
        public InProgress()
        {
            InitializeComponent();
        }

        public void LoadDrafts()
        {
            flowLayoutPanel1.Controls.Clear();

            var drafts = DraftHelper.LoadDrafts().ToList();
            drafts.Reverse();

            foreach (var draft in drafts)
            {
                if (draft.Original == null) continue;

                // Wrapper panel to hold card + delete button
                var wrapper = new Guna.UI2.WinForms.Guna2Panel
                {
                    Size = new Size(flowLayoutPanel1.Width - 25, 90),
                    BackColor = Color.Transparent,
                    BorderThickness = 0,
                    FillColor = Color.Transparent
                };

                // Delete button
                var deleteBtn = new Guna.UI2.WinForms.Guna2Button
                {
                    Text = "✕",
                    Size = new Size(30, 30),
                    FillColor = Color.FromArgb(180, 50, 70),
                    ForeColor = Color.White,
                    BorderRadius = 8,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Location = new Point(wrapper.Width - 35, 25)
                };

                var capturedDraft = draft;
                var capturedWrapper = wrapper;

                deleteBtn.Click += (s, e) =>
                {
                    var confirm = MessageBox.Show(
                        "Delete this draft?", "Confirm",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        DraftHelper.DeleteDraft(capturedDraft.EmailId);
                        flowLayoutPanel1.Controls.Remove(capturedWrapper);
                        capturedWrapper.Dispose();
                    }
                };

                if (draft.IsSingleCompose)
                {
                    var card = new SentEmailRow();
                    card.Location = new Point(0, 5);
                    card.SetDraft(draft);
                    card.SetCompactWidth(wrapper.Width - 50);
                    card.OnDraftClicked = () => OpenSingleDraft(capturedDraft);

                    deleteBtn.Location = new Point(wrapper.Width - 35, 25);

                    wrapper.Controls.Add(card);
                    wrapper.Controls.Add(deleteBtn);
                }
                else
                {
                    var card = new EmailRow();
                    card.Location = new Point(0, 5);
                    card.SetEmail(draft.Original, AppState.GmailService!);
                    card.SetCompactWidth(wrapper.Width - 50); // call this instead
                    card.OnDraftClicked = () => OpenDraft(capturedDraft);

                    deleteBtn.Location = new Point(wrapper.Width - 35, 25);

                    wrapper.Controls.Add(card);
                    wrapper.Controls.Add(deleteBtn);
                }

                flowLayoutPanel1.Controls.Add(wrapper);
            }
        }
        private void OpenDraft(DraftModel draft)
        {
            var home = Application.OpenForms.OfType<Home>().FirstOrDefault();
            if (home == null || draft.Original == null) return;

            var compose = new Compose();
            compose.LoadEmail(draft.Original, AppState.GmailService!);
            compose.LoadDraft(draft);
            home.ShowFullscreenCompose(compose);
        }

        private void OpenSingleDraft(DraftModel draft)
        {
            var compose = new SingleCompose(AppState.GmailService);
            compose.LoadDraft(draft);
            compose.ShowDialog();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void guna2vScrollBar1_Scroll(object sender, ScrollEventArgs e) { }
        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }
    }
}