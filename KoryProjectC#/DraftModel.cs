namespace KoryProjectC_
{
    public class DraftModel
    {
        public string EmailId { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Salutation { get; set; } = "";
        public string Body { get; set; } = "";
        public string Signature { get; set; } = "";
        public bool IsSingleCompose { get; set; } = false; // ADD THIS
        public EmailModel? Original { get; set; }
    }
}