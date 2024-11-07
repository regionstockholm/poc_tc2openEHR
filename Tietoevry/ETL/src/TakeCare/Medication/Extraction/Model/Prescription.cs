using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.Medication.Extraction.Extension;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    public class Prescription
    {
        public Guid Guid { get; set; }
        public Guid ParentGuid { get; set; }

        [XmlElement("TimestampSaved")]
        public string TimestampSaved { get; set; }

        [XmlElement("SavedByUserID")]
        public string SavedByUserID { get; set; }

        [XmlElement("SavedAtCareUnitID")]
        public string SavedAtCareUnitID { get; set; }

        [XmlElement("AdministrationRouteID")]
        public string AdministrationRouteID { get; set; }

        [XmlElement("AdministrationRouteText")]
        public string AdministrationRouteText { get; set; }

        [XmlElement("AdministrationTypeID")]
        public string AdministrationTypeID { get; set; }

        [XmlElement("AdministrationTypeText")]
        public string AdministrationTypeText { get; set; }

        [XmlElement("TreatmentReason")]
        public string TreatmentReason { get; set; }

        [XmlElement("TreatmentGoal")]
        public string TreatmentGoal { get; set; }

        [XmlElement("Instruction")]
        public string Instruction { get; set; }

        [XmlElement("ReviewDate")]
        public string ReviewDate { get; set; }

        [XmlElement("ReviewTime")]
        public string ReviewTime { get; set; }

        private string _fullReviewDate;
        public string FullReviewDate
        {
            get
            {
                return _fullReviewDate;
            }
            set
            {
                var reviewDate = string.Concat(ReviewDate, ReviewTime);
                _fullReviewDate = reviewDate.GetFormattedISODate();
            }
        }


        [XmlElement("ReviewDecisionByUserID")]
        public string ReviewDecisionByUserID { get; set; }

        [XmlElement("IsReplaceable")]
        public string IsReplaceable { get; set; }

        [XmlElement("DilutionLiquid")]
        public string DilutionLiquid { get; set; }

        [XmlElement("DilutionAmount")]
        public string DilutionAmount { get; set; }

        [XmlElement("CessationReasonID")]
        public string CessationReasonID { get; set; }

        [XmlElement("CessationReasonText")]
        public string CessationReasonText { get; set; }

        [XmlElement("IsStdSolution")]
        public string IsStdSolution { get; set; }

        [XmlElement("FirstDoseDate")]
        public string FirstDoseDate { get; set; }

        [XmlElement("FirstDoseTime")]
        public string FirstDoseTime { get; set; }

        private string _fullFirstDoseDate;
        public string FullFirstDoseDate { 
            get 
            {
                return _fullFirstDoseDate;  
            } 
            set
            { 
                var firstDose = string.Concat(FirstDoseDate, FirstDoseTime);
                _fullFirstDoseDate = firstDose.GetFormattedISODate();
            } 
        }

        [XmlElement("LastDoseDate")]
        public string LastDoseDate { get; set; }

        [XmlElement("LastDoseTime")]
        public string LastDoseTime { get; set; }

        private string _fullLastDoseDate;

        public string FullLastDoseDate
        {
            get
            {
                return _fullLastDoseDate;
            }
            set
            {
                var lastDose = string.Concat(LastDoseDate, LastDoseTime);
                _fullLastDoseDate = lastDose.GetFormattedISODate();
            }
        }

        [XmlElement("AdministrationOccasionID")]
        public string AdministrationOccasionID { get; set; }

        [XmlElement("AdministrationOccasionText")]
        public string AdministrationOccasionText { get; set; }

        [XmlElement("IsDispensionAllowed")]
        public string IsDispensionAllowed { get; set; }

        [XmlElement("SolutionStrength")]
        public string SolutionStrength { get; set; }

        [XmlElement("SolutionStrengthUnitID")]
        public string SolutionStrengthUnitID { get; set; }

        [XmlElement("SolutionStrengthUnitText")]
        public string SolutionStrengthUnitText { get; set; }
    }
}
