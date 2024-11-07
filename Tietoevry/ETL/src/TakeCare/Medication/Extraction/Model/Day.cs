using System.Xml.Serialization;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    public class Day
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

        [XmlElement("SignerUserID")]
        public string SignerUserID { get; set; }

        [XmlElement("AdministrationStartDate")]
        public string AdministrationStartDate { get; set; }

        [XmlElement("AdministrationStartTime")]
        public string AdministrationStartTime { get; set; }

        [XmlElement("MaxDailyDose")]
        public string MaxDailyDose { get; set; }

        [XmlElement("InfusionTime")]
        public string InfusionTime { get; set; }

        [XmlElement("IsSelfAdministered")]
        public string IsSelfAdministered { get; set; }

        [XmlElement("DosageInstruction")]
        public string DosageInstruction { get; set; }

        [XmlElement("DosageInstructionTemplate")]
        public string DosageInstructionTemplate { get; set; }
    }
}
