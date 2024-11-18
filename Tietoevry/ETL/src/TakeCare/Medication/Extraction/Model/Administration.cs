using System.Xml.Serialization;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    public class Administration
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

        [XmlElement("AsNeededNo")]
        public string AsNeededNo { get; set; }

        [XmlElement("Comment")]
        public string Comment { get; set; }

        [XmlElement("TreatmentReason")]
        public string TreatmentReason { get; set; }

        [XmlElement("AdministrationDatetime")]
        public string AdministrationDatetime { get; set; }

        [XmlElement("PrescriptionDate")]
        public string PrescriptionDate { get; set; }

        [XmlElement("PrescriptionTime")]
        public string PrescriptionTime { get; set; }

        private string _fullPrescriptionDate;
        public string FullPrescriptionDate
        {
            get
            {
                return _fullPrescriptionDate;
            }
            set
            {
                var fulldate = string.Concat(PrescriptionDate, PrescriptionTime);
                _fullPrescriptionDate = fulldate.GetFormattedISODate();
            }
        }

        [XmlElement("InfusionKey")]
        public string InfusionKey { get; set; }

        [XmlElement("OrderCreatedAtCareUnitID")]
        public string OrderCreatedAtCareUnitID { get; set; }

        [XmlElement("OrderDoseText")]
        public string OrderDoseText { get; set; }

        [XmlElement("OrderDoseTextSolution")]
        public string OrderDoseTextSolution { get; set; }


        [XmlArray("Preparations")]
        [XmlArrayItem("Preparation")]
        public List<Preparation> Preparations { get; set; }
    }

    public class Preparation
    {
        [XmlElement("DrugRow")]
        public string DrugRow { get; set; }

        [XmlElement("Dose")]
        public string Dose { get; set; }
    }
}