using System.Xml.Serialization;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Model
{
    public class Medication
    {
        public Guid Guid { get; set; }
        public List<Guid> ChildGuids { get; set; }

        [XmlElement("DocumentID")]
        public string DocumentID { get; set; }

        [XmlElement("ApprovedForPatient")]
        public string ApprovedForPatient { get; set; }

        [XmlElement("ParentDocumentID")]
        public string ParentDocumentID { get; set; }

        [XmlElement("TimestampSaved")]
        public string TimestampSaved { get; set; }

        [XmlElement("TimestampCreated")]
        public string TimestampCreated { get; set; }

        [XmlElement("SavedByUserID")]
        public string SavedByUserID { get; set; }

        [XmlElement("SignerUserID")]
        public string SignerUserID { get; set; }

        [XmlElement("SignedByUserID")]
        public string SignedByUserID { get; set; }

        [XmlElement("SignedDatetime")]
        public string SignedDatetime { get; set; }

        [XmlElement("RegistrationStatus")]
        public string RegistrationStatus { get; set; }

        [XmlElement("SavedAtCareUnitID")]
        public string SavedAtCareUnitID { get; set; }

        [XmlElement("CreatedAtCareUnitID")]
        public string CreatedAtCareUnitID { get; set; }

        [XmlElement("DatabaseID")]
        public string DatabaseID { get; set; }

        [XmlElement("IsMixture")]
        public string IsMixture { get; set; }

        [XmlElement("DosageType")]
        public string DosageType { get; set; }

        [XmlElement("ExternalStartDate")]
        public string ExternalStartDate { get; set; }

        private string _externalStartDate;
        public string ExternalISOStartDate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ExternalStartDate) && !ExternalStartDate.Contains("-"))
                {
                    _externalStartDate = ExternalStartDate.GetFormattedISODate();
                }
                else
                {
                    _externalStartDate = ExternalStartDate;

                }
                return _externalStartDate;
            }
        }

        [XmlElement("ExternalPrescriber")]
        public string ExternalPrescriber { get; set; }

        [XmlElement("ChangeReasonID")]
        public string ChangeReasonID { get; set; }

        [XmlElement("ChangeReasonText")]
        public string ChangeReasonText { get; set; }

        [XmlElement("HasOrdinationReason")]
        public string HasOrdinationReason { get; set; }

        [XmlElement("IsTriggeredByATC")]
        public string IsTriggeredByATC { get; set; }

        [XmlElement("ProfylaxID")]
        public string ProfylaxID { get; set; }

        [XmlElement("PrescriptionDocumentIDs")]
        public string PrescriptionDocumentIDs { get; set; }

        [XmlElement("Prescription")]
        public Prescription Prescription { get; set; }

        [XmlArray("Drugs")]
        [XmlArrayItem("Drug")]
        public List<Drug> Drugs { get; set; }

        [XmlArray("Dosages")]
        [XmlArrayItem("Dosage")]
        public List<Dosage> Dosage { get; set; }

        [XmlArray("Days")]
        [XmlArrayItem("Day")]
        public List<Day> Days { get; set; }


        private List<Administration> _administrations;

        [XmlArray("Administrations")]
        [XmlArrayItem("Administration")]
        public List<Administration> Administrations { 
            get
            {
            return _administrations;
            }
            set {
                if (value != null || value.Count > 0)
                    _administrations = value;
            }
        }

        private List<Infusion> _infusions;

        [XmlArray("Infusions")]
        [XmlArrayItem("Infusion")]
        public List<Infusion> Infusions  { 
            get
            {
            return _infusions;
            }
            set {
                if (value != null || value.Count > 0)
                    _infusions = value;
            }
        }
        
    }
}
