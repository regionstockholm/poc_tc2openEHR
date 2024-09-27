using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Extension;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Model.CareDoc
{

    [XmlRoot("X2Message")]
    public class XMLResponse
    {
        [XmlAttribute("MsgType")]
        public string MsgType { get; set; }

        private string time;

        [XmlAttribute("Time")]
        public string Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value.GetFormattedISODate();
            }
        }

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

        [XmlElement("CareDocumentation")]
        public CareDocumentation CareDocumentation { get; set; }
    }
}
