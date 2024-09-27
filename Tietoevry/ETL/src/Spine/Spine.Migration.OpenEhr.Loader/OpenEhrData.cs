using Newtonsoft.Json.Linq;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class OpenEhrData
    {
        public OpenEhrData()
        {
            Compositions = new List<JObject>();
        }
        public string PatientID { get; set; }
        public List<JObject> Compositions { get; set; }
    }
}
