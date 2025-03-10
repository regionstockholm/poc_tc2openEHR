namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class TcMedicationOrder
    {
        private const string _prefix = "läkemedelsförskrivning";
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
                        ""{_prefix}/läkemedelsbeställning/_uid"": ""{Guid}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/timing"": ""R2/2024-10-14T15:00:00Z/P3M"",
                        ""{_prefix}/läkemedelsbeställning/order:0/timing|formalism"": ""timing"",
                        ""{_prefix}/läkemedelsbeställning/narrative"": ""Human readable instruction narrative"",
                        ""{_prefix}/läkemedelsbeställning/language|code"": ""sv"",
                        ""{_prefix}/läkemedelsbeställning/language|terminology"": ""ISO_639-1"",
                        ""{_prefix}/läkemedelsbeställning/encoding|code"": ""UTF-8"",
                        ""{_prefix}/läkemedelsbeställning/encoding|terminology"": ""IANA_character-sets"",
                        ""{_prefix}/composer/_identifier:0|name"":  ""Signing user"",
                        ""{_prefix}/composer/_identifier:0|id"" :  ""{SignedByUserID}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/läkemedelsdatabas_id"": ""{DatabaseID}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/är_mixtur"": ""{IsMixture}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/externt_startdatum"": ""{ExternalStartDate}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/extern_förskrivare"": ""{ExternalPrescriber}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/har_ordinationsorsak"": {HasOrdinationReason},
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/triggas_av_atc"": {IsTriggeredByATC},
                        ""{_prefix}/context/förskrivnings_identifierare:1"": ""{PrescriptionDocumentIDs}"",";
            
            var dosageTypeAql = string.IsNullOrWhiteSpace(DosageType)  ? string.Empty :
                            $@"""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/doseringstyp|code"": ""{DosageType}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/doseringstyp|value"": ""{(DosageTypeEnum)Convert.ToInt16(DosageType)}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/doseringstyp|terminology"": ""TC-Dosage-Type"",";

            if (!string.IsNullOrWhiteSpace(dosageTypeAql))
            {
                result = $@"{result}
                          {dosageTypeAql}";
            }

            var changeReasonAql = (string.IsNullOrWhiteSpace(ChangeReasonID) || string.IsNullOrWhiteSpace(ChangeReasonText)) ? string.Empty :
                            $@"""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/orsak_till_ändring|code"": ""{ChangeReasonID}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/orsak_till_ändring|value"": ""{ChangeReasonText}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/orsak_till_ändring|terminology"": ""TC-Meds-Change-Reason"",";

            if (!string.IsNullOrWhiteSpace(changeReasonAql))
            {
                result = $@"{result}
                          {changeReasonAql}";
            }

            var profylaxIDAql = string.IsNullOrWhiteSpace(ProfylaxID) ? string.Empty :
                            $@"""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/profylax|code"": ""{ProfylaxID}"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/profylax|value"": ""Perioperable"",
                        ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/profylax|terminology"": ""TC-Prophylaxis"",";

            if (!string.IsNullOrWhiteSpace(profylaxIDAql))
            {
                result = $@"{result}
                          {profylaxIDAql}";
            }

            var registrationStatus = string.IsNullOrWhiteSpace(RegistrationStatus) ? string.Empty :
                            $@"""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/registreringsstatus|code"": ""{RegistrationStatus}"",
                                ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/registreringsstatus|value"": ""{(RegistrationStatusEnum)Convert.ToInt16(RegistrationStatus)}"",
                                ""{_prefix}/läkemedelsbeställning/order:0/medicin_extension/registreringsstatus|terminology"": ""TC-Medication-RegistrationStatus"",";

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
        Cancelled = -2 //means that the prescription has never been administered
    }
    enum DosageTypeEnum
    {
        Rb = 1, // 1 = continuous (each day)
        Vb = 2, //2 = if necessary 
        Tf = 3, //3 = temporary (one-time prescription)
        Sch = 4, //4 = according to schedule
        Bhs = 5 //5 = according to Treatment schedule
    }
}
