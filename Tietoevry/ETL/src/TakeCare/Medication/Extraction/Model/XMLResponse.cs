using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    [XmlRoot("X2Message")]
    public class XMLResponse
    {
        [XmlAttribute("MsgType")]
        public string MsgType { get; set; }

        private string _time { get; set; }

        [XmlAttribute("Time")]
        public string Time { get { return _time; } set { _time = value.GetFormattedISODate(); } }

        [XmlAttribute("User")]
        public string User { get; set; }

        [XmlAttribute("CareUnitIdType")]
        public string CareUnitIdType { get; set; }

        [XmlAttribute("CareUnitId")]
        public string CareUnitId { get; set; }

        [XmlAttribute("Method")]
        public string Method { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("Conga")]
        public string Conga { get; set; }

        [XmlAttribute("MsgId")]
        public string MsgId { get; set; }

        [XmlAttribute("SystemId")]
        public string SystemId { get; set; }

        [XmlElement("PatientID")]
        public Patient PatientId { get; set; }

        [XmlArray("Medications")]
        [XmlArrayItem("Medication")]
        public List<Medication> Medications { get; set; }
    }

    public class Patient
    {
        [XmlElement("ID")]
        public string Id { get; set; }
    }
}
