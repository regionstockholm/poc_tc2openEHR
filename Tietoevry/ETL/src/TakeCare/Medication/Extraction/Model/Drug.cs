using System.Xml.Serialization;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    public class Drug
    {
        public Guid Guid { get; set; }
        public Guid ParentGuid { get; set; }

        [XmlElement("Row")]
        public string Row { get; set; }

        [XmlElement("PreparationText")]
        public string PreparationText { get; set; }

        [XmlElement("DrugCode")]
        public string DrugCode { get; set; }

        [XmlElement("SpecialityID")]
        public string SpecialityID { get; set; }

        [XmlElement("SpecialDrugCode")]
        public string SpecialDrugCode { get; set; }

        [XmlElement("DrugID")]
        public string DrugID { get; set; }

        [XmlElement("DoseForm")]
        public string DoseForm { get; set; }

        [XmlElement("DoseFormCode")]
        public string DoseFormCode { get; set; }

        [XmlElement("ATCCode")]
        public string ATCCode { get; set; }
        
        [XmlElement("ATCName")]
        public string ATCName { get; set; }

        [XmlElement("Strength")]
        public string Strength { get; set; }

        [XmlElement("StrengthUnit")]
        public string StrengthUnit { get; set; }

        [XmlElement("InternalArticleStrength")]
        public string InternalArticleStrength { get; set; }

        [XmlElement("UnitCode")]
        public string UnitCode { get; set; }

        [XmlElement("DosageUnitID")]
        public string DosageUnitID { get; set; }

        [XmlElement("DosageUnitText")]
        public string DosageUnitText { get; set; }

        [XmlElement("StdSolutionAmount")]
        public string StdSolutionAmount { get; set; }

        [XmlElement("ProductType")]
        public string ProductType { get; set; }

        [XmlElement("IsApproved")]
        public string IsApproved { get; set; }
    }
}
