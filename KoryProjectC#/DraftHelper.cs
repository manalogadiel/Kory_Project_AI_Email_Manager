using System.Text.Json;

namespace KoryProjectC_
{
    public static class DraftHelper
    {
        private static readonly string DraftPath = Path.Combine(
            Application.StartupPath, "drafts.json");

        public static List<DraftModel> LoadDrafts()
        {
            if (!File.Exists(DraftPath)) return new List<DraftModel>();
            try
            {
                var json = File.ReadAllText(DraftPath);
                return JsonSerializer.Deserialize<List<DraftModel>>(json)
                       ?? new List<DraftModel>();
            }
            catch { return new List<DraftModel>(); }
        }

        public static void SaveDraft(DraftModel draft)
        {
            var drafts = LoadDrafts();

            // Replace if already exists for same email
            var existing = drafts.FirstOrDefault(d => d.EmailId == draft.EmailId);
            if (existing != null) drafts.Remove(existing);

            drafts.Add(draft);
            File.WriteAllText(DraftPath, JsonSerializer.Serialize(drafts));
        }

        public static void DeleteDraft(string emailId)
        {
            var drafts = LoadDrafts();
            drafts.RemoveAll(d => d.EmailId == emailId);
            File.WriteAllText(DraftPath, JsonSerializer.Serialize(drafts));
        }
    }
}