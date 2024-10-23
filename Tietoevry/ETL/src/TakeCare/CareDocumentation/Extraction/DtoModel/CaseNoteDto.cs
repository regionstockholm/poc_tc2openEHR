using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Model.CareDoc;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel
{
    public class CaseNoteDto
    {
        public string DocumentId { get; set; }

        public string DocCreatedByUserId { get; set; }

        public string DocCreatedByProfessionId { get; set; }

        public string DocCreatedAtCareUnitId { get; set; }

        public string DocCreatedTimestamp { get; set; }

        public string DocSavedByUSerId { get; set; }

        public string DocSavedTimestamp { get; set; }

        public string SignerId { get; set; }

        public string CounterSignerId { get; set; }

        public string SignedById { get; set; }

        public string SignedTimestamp { get; set; }

        public string ApprovedForPatient { get; set; }

        public string EventDateTime { get; set; }

        public string DocumentTitle { get; set; }

        public string TemplateName { get; set; }

        public string TemplateId { get; set; }

        public string DocumentCode { get; set; }

        public string HeaderTerm { get; set; }

        public List<KeywordDto> Keywords { get; set; }
    }

    public class KeywordDto
    {
        public string Name { get; set; }

        public Guid Guid { get; set; }

        public Guid ParentId { get; set; }

        public List<Guid> Children { get; set; }

        public int ParentCount { get; set; }

        public string TermId { get; set; }
        public string Comment { get; set; }
        public KeywordValue? Value { get; set; }

    }
}
