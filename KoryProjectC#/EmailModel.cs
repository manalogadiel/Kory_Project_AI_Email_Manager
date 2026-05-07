namespace KoryProjectC_
{
    public class EmailModel
    {
        public string Id { get; set; } = "";
        public string ThreadId { get; set; } = "";
        public string Subject { get; set; } = "(No Subject)";
        public string FromName { get; set; } = "Unknown";
        public string FromEmail { get; set; } = "";
        public string Snippet { get; set; } = "";
        public string BodyHtml { get; set; } = "";
        public string BodyText { get; set; } = "";
        public string Date { get; set; } = "";
        public string Category { get; set; } = "NON-ACADEMIC";
        public bool IsRead { get; set; } = true;
    }
}