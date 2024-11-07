namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
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
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/timing"": ""R2/2024-10-14T15:00:00Z/P3M"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/timing|formalism"": ""timing"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/narrative"": ""Human readable instruction narrative"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/language|code"": ""sv"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/language|terminology"": ""ISO_639-1"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/encoding|code"": ""UTF-8"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/encoding|terminology"": ""IANA_character-sets"",
                        ""läkemedelsförskrivning/category|code"": ""433"",
                        ""läkemedelsförskrivning/category|value"": ""event"",
                        ""läkemedelsförskrivning/category|terminology"": ""openehr"",
                        ""läkemedelsförskrivning/language|code"": ""sv"",
                        ""läkemedelsförskrivning/language|terminology"": ""ISO_639-1"",
                        ""läkemedelsförskrivning/territory|code"": ""SV"",
                        ""läkemedelsförskrivning/territory|terminology"": ""ISO_3166-1"",";
            return result;

            //todo once got confirmation on this query
            /*""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/läkemedelsdatabas_id"": ""{DatabaseID}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/är_mixtur"": ""{IsMixture}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/doseringstyp|code"": ""{DosageType}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/doseringstyp|value"": ""{DosageTypeValue}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/doseringstyp|terminology"": ""TC-Dosage-Type"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/externt_startdatum"": ""{ExternalStartDate}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/extern_förskrivare"": ""{ExternalPrescriber}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/orsak_till_ändring|code"": ""{ChangeReasonID}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/orsak_till_ändring|value"": ""{ChangeReasonText}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/orsak_till_ändring|terminology"": ""TC-Meds-Change-Reason"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/har_ordinationsorsak"": {HasOrdinationReason},
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/triggas_av_atc"": {IsTriggeredByATC},
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/profylax|code"": ""{ProfylaxID}"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/profylax|value"": ""Perioperable"",
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/medicin_extension_rsk/profylax|terminology"": ""TC-Prophylaxis"",*/
        }
    }
}
