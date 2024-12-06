using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    public class Dosage
    {
        public Guid Guid { get; set; }
        public Guid ParentGuid { get; set; }

        [XmlElement("DosageID")]
        public string DosageID { get; set; }

        [XmlElement("TimestampSaved")]
        public string TimestampSaved { get; set; }

        [XmlElement("SavedByUserID")]
        public string SavedByUserID { get; set; }

        [XmlElement("SavedAtCareUnitID")]
        public string SavedAtCareUnitID { get; set; }

        [XmlElement("StartDate")]
        public string StartDate { get; set; }

        [XmlElement("StartTime")]
        public string StartTime { get; set; }

        private string _fullStartDate;

        public string FullStartDate 
        {
            get
            {
                var lastDose = string.Concat(StartDate, StartTime);
                _fullStartDate = lastDose.GetFormattedISODate();
                return _fullStartDate;
            }
        }

        [XmlElement("ScheduleType")]
        public string ScheduleType { get; set; }

        [XmlElement("Period")]
        public string Period { get; set; }

        [XmlElement("IsGivenOnMondays")]
        public string IsGivenOnMondays { get; set; }

        [XmlElement("IsGivenOnTuesdays")]
        public string IsGivenOnTuesdays { get; set; }

        [XmlElement("IsGivenOnWednesdays")]
        public string IsGivenOnWednesdays { get; set; }

        [XmlElement("IsGivenOnThursdays")]
        public string IsGivenOnThursdays { get; set; }

        [XmlElement("IsGivenOnFridays")]
        public string IsGivenOnFridays { get; set; }

        [XmlElement("IsGivenOnSaturdays")]
        public string IsGivenOnSaturdays { get; set; }

        [XmlElement("IsGivenOnSundays")]
        public string IsGivenOnSundays { get; set; }

        [XmlArray("DosageDrugs")]
        [XmlArrayItem("DosageDrug")]
        public List<DosageDrug> DosageDrugs { get; set; }
    }

    public class DosageDrug
    {
        public Guid Guid { get; set; }
        public Guid ParentGuid { get; set; }

        [XmlElement("DrugRow")]
        public string DrugRow { get; set; }

        [XmlElement("DrugCode")]
        public string DrugCode { get; set; }

        [XmlElement("DoseText")]
        public string DoseText { get; set; }

        [XmlElement("DoseNumerical")]
        public string DoseNumerical { get; set; }
    }
}
