namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class Created
    {
        public UserData User { get; set; }
        public string? DateTime { get; set; }
        public CareUnit CareUnit { get; set; }
    }
}
