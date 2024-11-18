namespace TakeCare.Foundation.OpenEhr.Models.Model
{
    public class TcMedicationOrder
    {
        public Guid Guid { get; set; }
        public List<Guid> ChildGuids { get; set; }
        public string DocumentID { get; set; }
        public string ParentDocumentID { get; set; }
        public string TimestampSaved { get; set; }
        public string TimestampCreated { get; set; }
        public string SavedByUserID { get; set; }
        public string SignerUserID { get; set; }
        public string SignedByUserID { get; set; }
        public string SignedDatetime { get; set; }
        public string RegistrationStatus { get; set; }
        public string SavedAtCareUnitID { get; set; }
        public string CreatedAtCareUnitID { get; set; }
        public string DatabaseID { get; set; }
        public string IsMixture { get; set; }
        public string DosageType { get; set; }
        public string DosageTypeValue { get; set; }
        public string DosageTypeTerm { get; set; }
        public string ExternalStartDate { get; set; }
        public string ExternalPrescriber { get; set; }
        public string ChangeReasonID { get; set; }
        public string ChangeReasonText { get; set; }
        public string HasOrdinationReason { get; set; }
        public string IsTriggeredByATC { get; set; }
        public string ProfylaxID { get; set; }
        public string PrescriptionDocumentIDs { get; set; }
        public override string ToString()
        {
            var result = $@"
                        ""läkemedelsförskrivning/läkemedelsbeställning/_uid"": ""{Guid}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/timing"": ""R2/2024-10-14T15:00:00Z/P3M"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/timing|formalism"": ""timing"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/narrative"": ""Human readable instruction narrative"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/language|code"": ""sv"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/language|terminology"": ""ISO_639-1"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/encoding|code"": ""UTF-8"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/encoding|terminology"": ""IANA_character-sets"",
                        ""läkemedelsförskrivning/composer|name"":  ""Signing user"",
                        ""läkemedelsförskrivning/composer/identifiers:0|id"" :  ""{SignedByUserID}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/läkemedelsdatabas_id"": ""{DatabaseID}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/är_mixtur"": ""{IsMixture}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/externt_startdatum"": ""{ExternalStartDate}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/extern_förskrivare"": ""{ExternalPrescriber}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/har_ordinationsorsak"": {HasOrdinationReason},
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/triggas_av_atc"": {IsTriggeredByATC},
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/har_ordinationsorsak"": {HasOrdinationReason},
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/triggas_av_atc"": {IsTriggeredByATC},
                        ""läkemedelsförskrivning/context/förskrivnings_identifierare:1"": ""{PrescriptionDocumentIDs}"",";
            
            var dosageTypeAql = (string.IsNullOrWhiteSpace(DosageType) || string.IsNullOrWhiteSpace(DosageTypeValue)) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/doseringstyp|code"": ""{DosageType}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/doseringstyp|value"": ""{DosageTypeValue}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/doseringstyp|terminology"": ""TC-Dosage-Type"",";

            if (!string.IsNullOrWhiteSpace(dosageTypeAql))
            {
                result = $@"{result}
                          {dosageTypeAql}";
            }

            var changeReasonAql = (string.IsNullOrWhiteSpace(ChangeReasonID) || string.IsNullOrWhiteSpace(ChangeReasonText)) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/orsak_till_ändring|code"": ""{ChangeReasonID}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/orsak_till_ändring|value"": ""{ChangeReasonText}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/orsak_till_ändring|terminology"": ""TC-Meds-Change-Reason"",";

            if (!string.IsNullOrWhiteSpace(changeReasonAql))
            {
                result = $@"{result}
                          {changeReasonAql}";
            }

            var profylaxIDAql = string.IsNullOrWhiteSpace(ProfylaxID) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/profylax|code"": ""{ProfylaxID}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/profylax|value"": ""Perioperable"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/profylax|terminology"": ""TC-Prophylaxis"",";

            if (!string.IsNullOrWhiteSpace(profylaxIDAql))
            {
                result = $@"{result}
                          {profylaxIDAql}";
            }

            var registrationStatus = string.IsNullOrWhiteSpace(RegistrationStatus) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/registreringsstatus|code"": ""{RegistrationStatus}"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/registreringsstatus|value"": ""{(RegistrationStatusEnum)Convert.ToInt16(RegistrationStatus)}"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/registreringsstatus|terminology"": ""TC-Medication-RegistrationStatus"",";

            if (!string.IsNullOrWhiteSpace(registrationStatus))
            {
                result = $@"{result}
                          {registrationStatus}";
            }

            return result;            
        }
    }

    enum RegistrationStatusEnum
    {
        Normal,
        Cancelled = -2
    }
}
