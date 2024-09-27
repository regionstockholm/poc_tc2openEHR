namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel
{
    public class CareDocumentationDto
    {
        public string PatientId { get; set; }

        public List<CaseNoteDto> CaseNotes { get; set; }
    }
}
