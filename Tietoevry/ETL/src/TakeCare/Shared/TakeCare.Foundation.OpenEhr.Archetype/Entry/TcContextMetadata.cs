namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcContextMetadata
    {
        private const string  _prefix = "vårdkontakt";
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
        public string SavedByFullName { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string CreatedOnDate { get; set; }
        public string Heading { get; set; }
        public string LinkCode { get;  set; }
        public string VersionId { get;  set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }

        public override string ToString()
        {

            var result = $@"
                             ""{_prefix}/context/metadata/dokument_id"": """",
                             ""{_prefix}/context/metadata/dokumentationsmall|code"": ""{TemplateId}"",
                             ""{_prefix}/context/metadata/dokumentationsmall|value"": ""{TemplateName}"",
                             ""{_prefix}/context/metadata/dokumentationsmall|terminology"": ""TC-Template-codes"",
                             ""{_prefix}/context/metadata/överordnat_dokument_id"": """",
                             ""{_prefix}/context/metadata/godkänd_för_patient"": true,
                             ""{_prefix}/context/metadata/dokumentskaparens_användar_id"": ""{CreatedByUserId}"",
                             ""{_prefix}/context/metadata/dokumentskaparens_användar_id|issuer"": ""{Issuer}"",
                             ""{_prefix}/context/metadata/dokumentskaparens_användar_id|assigner"": ""{Assigner}"",
                             ""{_prefix}/context/metadata/dokument_sparat_av_fullständigt_namn"": ""{SavedByFullName}"",
                             ""{_prefix}/context/metadata/dokumentskaparens_användar_id|type"": ""UserId"",
                             ""{_prefix}/context/metadata/dokument_sparat_av_användar_id"": ""{SavedByUserId}"",
                             ""{_prefix}/context/metadata/dokument_sparat_av_användar_id|issuer"": ""{Issuer}"",
                             ""{_prefix}/context/metadata/dokument_sparat_av_användar_id|assigner"": ""{Assigner}"",
                             ""{_prefix}/context/metadata/dokument_sparat_av_användar_id|type"": ""UserId"",
                             ""{_prefix}/context/metadata/dokument_skapat_på_vårdenhet_namn"": ""{CareUnitName}"",
                             ""{_prefix}/context/metadata/dokument_skapat_på_vårdenhet_id"": ""{CareUnitId}"",
                             ""{_prefix}/context/metadata/dokument_sparat_på_vårdenhet_id"": ""{CareUnitId}"",
                             ""{_prefix}/context/metadata/tidsstämpel_för_sparat_dokument"": ""{SavedOn}"",
                             ""{_prefix}/context/metadata/signeringstidpunkt"": """",
                             ""{_prefix}/context/metadata/signerare_id"": """",
                             ""{_prefix}/context/metadata/signerare_id|issuer"": ""{Issuer}"",
                             ""{_prefix}/context/metadata/signerare_id|assigner"": ""{Assigner}"",
                             ""{_prefix}/context/metadata/signerare_id|type"": ""UserId"",
                             ""{_prefix}/context/metadata/signerat_av_id"": """",
                             ""{_prefix}/context/metadata/signerat_av_id|issuer"": ""{Issuer}"",
                             ""{_prefix}/context/metadata/signerat_av_id|assigner"": ""{Assigner}"",
                             ""{_prefix}/context/metadata/signerat_av_id|type"": ""UserId"",
                             ""{_prefix}/context/metadata/kontrasignerare_id"": """",
                             ""{_prefix}/context/metadata/kontrasignerare_id|issuer"": ""{Issuer}"",
                             ""{_prefix}/context/metadata/kontrasignerare_id|assigner"": ""{Assigner}"",
                             ""{_prefix}/context/metadata/kontrasignerare_id|type"": ""UserId"",
                             ""{_prefix}/context/metadata/rubriktext"": """",
                             ""{_prefix}/context/metadata/dokumentationskod"": ""{DocumentCode}"",
	                         ""{_prefix}/context/metadata/dokumentnamn"": ""{DocumentName}"",
                             ""{_prefix}/context/metadata/dokumentationstidpunkt"":""{CreatedOnDate}"",
                             ""{_prefix}/_name"": ""{Heading}"",
                             ""{_prefix}/context/metadata/länkkod"": ""{LinkCode}"",
                             ""{_prefix}/context/metadata/versionsid"": ""{VersionId}"",";
                                        
            return result;

        }
    }
}
