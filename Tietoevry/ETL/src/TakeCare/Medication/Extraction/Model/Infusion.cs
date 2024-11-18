using System.Xml.Serialization;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    public class Infusion
    {
        public Guid Guid { get; set; }
        public Guid ParentGuid { get; set; }

        [XmlElement("Row")]
        public string Row { get; set; }

        [XmlElement("TimestampSaved")]
        public string TimestampSaved { get; set; }

        [XmlElement("SavedByUserID")]
        public string SavedByUserID { get; set; }

        [XmlElement("SavedAtCareUnitID")]
        public string SavedAtCareUnitID { get; set; }

        [XmlElement("EventDatetime")]
        public string EventDatetime { get; set; }

        [XmlElement("EventType")]
        public string EventType { get; set; }

        [XmlElement("Rate")]
        public string Rate { get; set; }

        [XmlElement("RateUnit")]
        public string RateUnit { get; set; }

        [XmlElement("TotalAmount")]
        public string TotalAmount { get; set; }

        [XmlElement("Comment")]
        public string Comment { get; set; }

        [XmlElement("PrescriptionDate")]
        public string PrescriptionDate { get; set; }

        [XmlElement("PrescriptionTime")]
        public string PrescriptionTime { get; set; }

        private string _fullPrescriptionDate;

        public string FullPrescriptionDate { get 
            {
                return _fullPrescriptionDate;
            }
            set {
                var fulldate = string.Concat(PrescriptionDate, PrescriptionTime);
                _fullPrescriptionDate = fulldate.GetFormattedISODate();
            }
        }

        [XmlElement("AdministrationKey")]
        public string AdministrationKey { get; set; }

        [XmlElement("OrderCreatedAtCareUnitID")]
        public string OrderCreatedAtCareUnitID { get; set; }
    }
}