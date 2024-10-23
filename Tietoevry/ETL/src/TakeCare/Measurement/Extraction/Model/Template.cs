namespace TakeCare.Migration.OpenEhr.Measurement.Extraction.Model
{
    public class Template
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PublishedDateTime { get; set; }
        public List<Terms> Terms { get; set; }
        public string ValidToDate { get; set; }
        public bool IsAddingTermsAllowed { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsTimeSetByDefault { get; set; }
        public bool IsTriageTemplate { get; set; }
    }

    public class Terms
    {
        public bool IsMandatory { get; set; }
        public TemplateTerm Term { get; set; }
    }
    public class TemplateTerm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }
}