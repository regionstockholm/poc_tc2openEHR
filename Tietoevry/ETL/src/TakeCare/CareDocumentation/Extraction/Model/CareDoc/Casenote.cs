using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Extension;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Model.CareDoc
{
    public class Casenote
    {
        [XmlElement("DocumentId")]
        public string DocumentId { get; set; }

        [XmlElement("DocCreatedByUserId")]
        public string DocCreatedByUserId { get; set; }

        [XmlElement("DocCreatedByProfessionId")]
        public string DocCreatedByProfessionId { get; set; }

        [XmlElement("DocCreatedAtCareUnitId")]
        public string DocCreatedAtCareUnitId { get; set; }

        private string docCreatedTimestamp;
        
        [XmlElement("DocCreatedTimestamp")]
        public string DocCreatedTimestamp {
            get
            {
                return docCreatedTimestamp;
            }
            set 
            {
                docCreatedTimestamp = value.GetFormattedISODate();
            } 
        }

        [XmlElement("DocSavedByUSerId")]
        public string DocSavedByUSerId { get; set; }

        private string docSavedTimestamp;

        [XmlElement("DocSavedTimestamp")]
        public string DocSavedTimestamp
        {
            get
            {
                return docSavedTimestamp;
            }
            set
            {
                docSavedTimestamp = value.GetFormattedISODate();
            }
        }
        
        [XmlElement("SignerId")]
        public string SignerId { get; set; }

        [XmlElement("SignedById")]
        public string SignedById { get; set; }

        private string signedTimestamp;

        [XmlElement("SignedTimestamp")]
        public string SignedTimestamp
        {
            get
            {
                return signedTimestamp;
            }
            set
            {
                signedTimestamp = value.GetFormattedISODate();
            }
        }

        [XmlElement("ApprovedForPatient")]
        public string ApprovedForPatient { get; set; }


        private string eventDateTime;


        [XmlElement("EventDateTime")]
        public string EventDateTime
        {
            get
            {
                return eventDateTime;
            }
            set
            {
                eventDateTime = value.GetFormattedISODate();
            }
        }

        [XmlElement("DocumentTitle")]
        public string DocumentTitle { get; set; }

        [XmlElement("TemplateName")]
        public string TemplateName { get; set; }

        [XmlElement("TemplateId")]
        public string TemplateId { get; set; }

        [XmlElement("DocumentCode")]
        public string DocumentCode { get; set; }

        [XmlElement("HeaderTerm")]
        public string HeaderTerm { get; set; }

        [XmlElement("Content")]
        public Content Content { get; set; }
    }

    public class Content
    {
        [XmlElement("Keyword")]
        public Keyword Keyword { get; set; }
    }
}
