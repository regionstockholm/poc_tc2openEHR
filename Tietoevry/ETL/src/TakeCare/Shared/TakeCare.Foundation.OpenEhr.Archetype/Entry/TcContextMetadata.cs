namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcContextMetadata
    {
        public string CareUnitName { get; set; } = "CareUnitName";
        public string CareUnitId { get; set; } = "CareUnitID";
        public string Issuer { get; set; } = "RSK";
        public string Assigner { get; set; } = "RSK";
        public string Code { get; set; } = "43741000";
        public string Value { get; set; } = "vårdenhet";
        public string CareProviderId { get; set; } = "CareProviderId";
        public string OrgId { get; set; } = "OrgId";
        public string CareGiverCode { get; set; } = "143591000052106";
        public string CareGiverValue { get; set; } = "vårdgivare";
        public string SavedOn { get; set; }
        public string SavedByUserId { get; set; }
        public string CreatedByUserId { get; set; }
        public override string ToString()
        {

            var result = $@"
                             ""vårdkontakt/context/metadata/dokument_id"": """",
                             ""vårdkontakt/context/metadata/överordnat_dokument_id"": """",
                             ""vårdkontakt/context/metadata/godkänd_för_patient"": false,
                             ""vårdkontakt/context/metadata/dokumentskaparens_användar_id"": ""{CreatedByUserId}"",
                             ""vårdkontakt/context/metadata/dokumentskaparens_användar_id|issuer"": ""{Issuer}"",
                             ""vårdkontakt/context/metadata/dokumentskaparens_användar_id|assigner"": ""{Assigner}"",
                             ""vårdkontakt/context/metadata/dokumentskaparens_användar_id|type"": ""UserId"",
                             ""vårdkontakt/context/metadata/dokument_sparat_av_användar_id"": ""{SavedByUserId}"",
                             ""vårdkontakt/context/metadata/dokument_sparat_av_användar_id|issuer"": ""{Issuer}"",
                             ""vårdkontakt/context/metadata/dokument_sparat_av_användar_id|assigner"": ""{Assigner}"",
                             ""vårdkontakt/context/metadata/dokument_sparat_av_användar_id|type"": ""UserId"",
                             ""vårdkontakt/context/metadata/tidsstämpel_för_sparat_dokument"": ""{SavedOn}"",
                             ""vårdkontakt/context/metadata/signeringstidpunkt"": """",
                             ""vårdkontakt/context/metadata/signerare_id"": """",
                             ""vårdkontakt/context/metadata/signerare_id|issuer"": ""{Issuer}"",
                             ""vårdkontakt/context/metadata/signerare_id|assigner"": ""{Assigner}"",
                             ""vårdkontakt/context/metadata/signerare_id|type"": ""UserId"",
                             ""vårdkontakt/context/metadata/signerat_av_id"": """",
                             ""vårdkontakt/context/metadata/signerat_av_id|issuer"": ""{Issuer}"",
                             ""vårdkontakt/context/metadata/signerat_av_id|assigner"": ""{Assigner}"",
                             ""vårdkontakt/context/metadata/signerat_av_id|type"": ""UserId"",
                             ""vårdkontakt/context/metadata/kontrasignerare_id"": """",
                             ""vårdkontakt/context/metadata/kontrasignerare_id|issuer"": ""{Issuer}"",
                             ""vårdkontakt/context/metadata/kontrasignerare_id|assigner"": ""{Assigner}"",
                             ""vårdkontakt/context/metadata/kontrasignerare_id|type"": ""UserId"",
                             ""vårdkontakt/context/metadata/rubriktext"": """",";
                                        
            return result;

        }
    }
}
