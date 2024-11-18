namespace TakeCare.Foundation.OpenEhr.Models.Model
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
                        $@"""läkemedelsförskrivning/context/metadata/dokumentskaparens_användar_id"": ""{ParentDocId}"",
                            ""läkemedelsförskrivning/context/metadata/dokumentskaparens_användar_id|issuer"": ""Issuer"",
                            ""läkemedelsförskrivning/context/metadata/dokumentskaparens_användar_id|assigner"": ""Assigner"",
                            ""läkemedelsförskrivning/context/metadata/dokumentskaparens_användar_id|type"": ""Prescription"",";

            var result = $@"""läkemedelsförskrivning/context/metadata/dokument_id"": ""{DocumentId}"",
                            ""läkemedelsförskrivning/context/metadata/godkänd_för_patient"": ""{ApprovedForPatient}"",
                            ""läkemedelsförskrivning/context/metadata/dokumentationstidpunkt"": ""{CreatedOn}"",
                            ""läkemedelsförskrivning/context/metadata/dokument_skapat_på_vårdenhet_id"": ""{CreatedAtCareUnitId}"",
                            ""läkemedelsförskrivning/context/metadata/dokument_skapat_på_vårdenhet_id|issuer"": ""Issuer"",
                            ""läkemedelsförskrivning/context/metadata/dokument_skapat_på_vårdenhet_id|assigner"": ""Assigner"",
                            ""läkemedelsförskrivning/context/metadata/dokument_skapat_på_vårdenhet_id|type"": ""Prescription"",
                            ""läkemedelsförskrivning/context/metadata/dokument_skapat_på_vårdenhet_namn"": ""{CreatedAtCareUnitName}"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_av_användar_id"": ""{SavedByUserId}"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_av_användar_id|issuer"": ""Issuer"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_av_användar_id|assigner"": ""Assigner"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_av_användar_id|type"": ""Prescription"",
                            ""läkemedelsförskrivning/context/metadata/tidsstämpel_för_sparat_dokument"": ""{SavedOn}"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_på_vårdenhet_id"": ""{SavedAtCareUnitId}"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_på_vårdenhet_id|issuer"": ""Issuer"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_på_vårdenhet_id|assigner"": ""Assigner"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_på_vårdenhet_id|type"": ""Prescription"",
                            ""läkemedelsförskrivning/context/metadata/dokument_sparat_på_vårdenhet_namn"": ""{SavedAtCareUnitName}"",
                            ""läkemedelsförskrivning/context/metadata/signeringstidpunkt"": ""{SignedDateTime}"",
                            ""läkemedelsförskrivning/context/metadata/signerare_id"": ""{SignerUserID}"",
                            ""läkemedelsförskrivning/context/metadata/signerare_id|issuer"": ""Issuer"",
                            ""läkemedelsförskrivning/context/metadata/signerare_id|assigner"": ""Assigner"",
                            ""läkemedelsförskrivning/context/metadata/signerare_id|type"": ""Prescription"",
                            ""läkemedelsförskrivning/context/metadata/signerat_av_id"": ""{SignedByUserId}"",
                            ""läkemedelsförskrivning/context/metadata/signerat_av_id|issuer"": ""Issuer"",
                            ""läkemedelsförskrivning/context/metadata/signerat_av_id|assigner"": ""Assigner"",
                            ""läkemedelsförskrivning/context/metadata/signerat_av_id|type"": ""Prescription"",
                            ""läkemedelsförskrivning/context/förskrivnings_identifierare:0"": ""{PrescriptionDocumentIDs}"",
                            ""läkemedelsförskrivning/context/förskrivnings_identifierare:0|issuer"": ""Issuer"",
                            ""läkemedelsförskrivning/context/förskrivnings_identifierare:0|assigner"": ""Assigner"",
                            ""läkemedelsförskrivning/context/förskrivnings_identifierare:0|type"": ""Prescription"",";

            if (!string.IsNullOrWhiteSpace(docid))
                result = $@"{result}
                          {docid}";

            return result;
        }
    }
}
