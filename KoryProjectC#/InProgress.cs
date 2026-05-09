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

            var drafts = DraftHelper.LoadDrafts();
            drafts.Reverse();

            foreach (var draft in drafts)
            {
                if (draft.Original == null) continue;

                var card = new EmailRow();
                card.Width = flowLayoutPanel1.Width - 25;
                card.SetEmail(draft.Original, AppState.GmailService!);
                card.OnDraftClicked = () => OpenDraft(draft);
                flowLayoutPanel1.Controls.Add(card);
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

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }
    }
}