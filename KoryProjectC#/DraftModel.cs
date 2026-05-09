namespace KoryProjectC_
{
    public class DraftModel
    {
        public string EmailId { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Salutation { get; set; } = "";
        public string Body { get; set; } = "";
        public string Signature { get; set; } = "";
        public EmailModel? Original { get; set; }
    }
}