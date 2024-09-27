using Newtonsoft.Json.Linq;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class CareDocumentOpenEhrData
    {
        public CareDocumentOpenEhrData()
        {
            Compositions = new List<JObject>();
        }
        public string PatientID { get; set; }

        public List<JObject> Compositions { get; set; }
    }
}
