using Newtonsoft.Json.Linq;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class CareDocumentOpenEhrData
    {
        public CareDocumentOpenEhrData()
        {
            Compositions = new List<JObject>();
            CaseNotes = new List<OpenEhrCaseNote>();
        }
        public string PatientID { get; set; }

        public List<JObject> Compositions { get; set; }

        public List<OpenEhrCaseNote> CaseNotes { get; set; }

    }
}
