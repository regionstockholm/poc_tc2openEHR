using System.Collections.Generic;
using System.Xml.Serialization;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Model.CareDoc
{
    public class CareDocumentation
    {
        [XmlElement("PatientId")]
        public string PatientId { get; set; }

        [XmlArray("CaseNotes")]
        [XmlArrayItem("Casenote")]
        public List<Casenote> CaseNotes { get; set; }
    }
}
