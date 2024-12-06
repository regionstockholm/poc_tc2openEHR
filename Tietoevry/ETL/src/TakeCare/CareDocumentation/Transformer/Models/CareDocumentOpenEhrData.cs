using TakeCare.Migration.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class CareDocumentOpenEhrData
    {
        public CareDocumentOpenEhrData()
        {
            CaseNotes = new List<OpenEhrCaseNote>();
        }
        public string PatientID { get; set; }

        public List<OpenEhrCaseNote> CaseNotes { get; set; }

    }
}
