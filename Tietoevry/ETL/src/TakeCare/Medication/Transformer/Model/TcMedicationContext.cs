namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class TcMedicationContext
    {
        private const string _prefix = "läkemedelsförskrivning/context";
        public string Issuer { get; set; } = "Issurer";
        public string Assigner { get; set; } = "Assigner";
        public string Type { get; set; } = "Prescription";
        public Guid PrescriptionGuid { get; set; }
        public string PrescriptionDocumentIDs { get; set; }
        public string Prescription { get; set; }
        public string DocumentId { get; set; }
        public string ParentDocId { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedByUsedId { get; set; }
        public string CreatedAtCareUnitId { get; set; }
        public string CreatedAtCareUnitName { get; set; }
        public string SavedByUserId { get; set; }
        public string SavedOn { get; set; }
        public string SavedAtCareUnitId { get; set; }
        public string SavedAtCareUnitName { get; set; }
        public string SignedDateTime { get; set; }
        public string SignerUserID { get; set; }
        public string SignedByUserId { get; set; }
        public string ApprovedForPatient { get; set; }

        public override string ToString()
        {
            var docid = string.IsNullOrWhiteSpace(ParentDocId) ? string.Empty : 
                        $@"""{_prefix}/metadata/dokumentskaparens_användar_id"": ""{ParentDocId}"",
                            ""{_prefix}/metadata/dokumentskaparens_användar_id|issuer"": ""Issuer"",
                            ""{_prefix}/metadata/dokumentskaparens_användar_id|assigner"": ""Assigner"",
                            ""{_prefix}/metadata/dokumentskaparens_användar_id|type"": ""Prescription"",";

            var result = $@"""{_prefix}/metadata/dokument_id"": ""{DocumentId}"",
                            ""{_prefix}/metadata/godkänd_för_patient"": ""{ApprovedForPatient}"",
                            ""{_prefix}/metadata/dokumentationstidpunkt"": ""{CreatedOn}"",
                            ""{_prefix}/metadata/dokument_skapat_på_vårdenhet_id"": ""{CreatedAtCareUnitId}"",
                            ""{_prefix}/metadata/dokument_skapat_på_vårdenhet_id|issuer"": ""Issuer"",
                            ""{_prefix}/metadata/dokument_skapat_på_vårdenhet_id|assigner"": ""Assigner"",
                            ""{_prefix}/metadata/dokument_skapat_på_vårdenhet_id|type"": ""Prescription"",
                            ""{_prefix}/metadata/dokument_skapat_på_vårdenhet_namn"": ""{CreatedAtCareUnitName}"",
                            ""{_prefix}/metadata/dokument_sparat_av_användar_id"": ""{SavedByUserId}"",
                            ""{_prefix}/metadata/dokument_sparat_av_användar_id|issuer"": ""Issuer"",
                            ""{_prefix}/metadata/dokument_sparat_av_användar_id|assigner"": ""Assigner"",
                            ""{_prefix}/metadata/dokument_sparat_av_användar_id|type"": ""Prescription"",
                            ""{_prefix}/metadata/tidsstämpel_för_sparat_dokument"": ""{SavedOn}"",
                            ""{_prefix}/metadata/dokument_sparat_på_vårdenhet_id"": ""{SavedAtCareUnitId}"",
                            ""{_prefix}/metadata/dokument_sparat_på_vårdenhet_id|issuer"": ""Issuer"",
                            ""{_prefix}/metadata/dokument_sparat_på_vårdenhet_id|assigner"": ""Assigner"",
                            ""{_prefix}/metadata/dokument_sparat_på_vårdenhet_id|type"": ""Prescription"",
                            ""{_prefix}/metadata/dokument_sparat_på_vårdenhet_namn"": ""{SavedAtCareUnitName}"",
                            ""{_prefix}/metadata/signeringstidpunkt"": ""{SignedDateTime}"",
                            ""{_prefix}/metadata/signerare_id"": ""{SignerUserID}"",
                            ""{_prefix}/metadata/signerare_id|issuer"": ""Issuer"",
                            ""{_prefix}/metadata/signerare_id|assigner"": ""Assigner"",
                            ""{_prefix}/metadata/signerare_id|type"": ""Prescription"",
                            ""{_prefix}/metadata/signerat_av_id"": ""{SignedByUserId}"",
                            ""{_prefix}/metadata/signerat_av_id|issuer"": ""Issuer"",
                            ""{_prefix}/metadata/signerat_av_id|assigner"": ""Assigner"",
                            ""{_prefix}/metadata/signerat_av_id|type"": ""Prescription"",";

            if (!string.IsNullOrWhiteSpace(docid))
                result = $@"{result}
                          {docid}";

            return result;
        }
    }
}
