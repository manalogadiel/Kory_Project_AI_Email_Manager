using System.Windows.Forms;

namespace KoryProjectC_
{
    public partial class SentEmailRow : UserControl
    {
        public Action? OnDraftClicked;
        private DraftModel? _draft;

        public SentEmailRow()
        {
            InitializeComponent();
            AttachEvents(this);
        }

        public void SetDraft(DraftModel draft)
        {
            _draft = draft;
            guna2HtmlLabel1.Text = $"To: {draft.Original?.FromEmail ?? ""}";
            guna2HtmlLabel2.Text = draft.Subject;
            guna2HtmlLabel3.Text = DateTime.Now.ToString("ddd, h:mm tt");
        }

        private void AttachEvents(Control control)
        {
            control.MouseClick += OnRowClick;
            foreach (Control child in control.Controls)
                AttachEvents(child);
        }

        private void OnRowClick(object? sender, MouseEventArgs e)
        {
            OnDraftClicked?.Invoke();
        }

        private void SentEmailRow_Load(object sender, EventArgs e) { }
        private void rowPanel_Paint(object sender, PaintEventArgs e) { }
        private void guna2CirclePictureBox1_Click(object sender, EventArgs e) { }
    }
}