namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class TcChemistryContextMetadata
    {
        public string _prefix { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string DocumnetSavedTimestamp { get; set; }
        public CareUnitDetails DocumentCreatedAtCareUnit { get; set; }
        public CareUnitDetails DocumentSavedAtCareUnit { get; set; }
        public string? IsCopy { get; set; }
        public string VersionId { get; set; }
        public bool? IsInterrupted { get; set; }

        public TcChemistryContextMetadata(string prefix)
        {
            _prefix = prefix;
        }

        public override string ToString()
        {
            var result = $@"";

            if (!string.IsNullOrEmpty(DocumentName))
            {
               result += $@"
                ""{_prefix}/dokumentnamn"": ""{DocumentName}"",";
            }

            if (!string.IsNullOrEmpty(DocumentCode))
            {
                result += $@"
                ""{_prefix}/dokumentationskod"": ""{DocumentCode}"",";
            }

            if (!string.IsNullOrEmpty(DocumnetSavedTimestamp))
            {
                result += $@"
                ""{_prefix}/tidsstämpel_för_sparat_dokument"": ""{DocumnetSavedTimestamp}"",";
            }

            if (IsCopy!=null)
            {
                result += $@"
                ""{_prefix}/är_kopia"": {IsCopy},";
            }

            if (!string.IsNullOrEmpty(VersionId))
            {
                result += $@"
                ""{_prefix}/versionsid"": ""{VersionId}"",";
            }

            if (IsInterrupted!=null)
            {
                result += $@"
                ""{_prefix}/äravbruten"": ""{IsInterrupted}"",";
            }
            if (DocumentCreatedAtCareUnit != null)
            {
               result += $@"
                ""{_prefix}/dokument_skapat_på_vårdenhet_id"": ""{DocumentCreatedAtCareUnit.Id}"",
                ""{_prefix}/dokument_skapat_på_vårdenhet_id|issuer"": ""{DocumentCreatedAtCareUnit.Issuer}"",
                ""{_prefix}/dokument_skapat_på_vårdenhet_id|assigner"": ""{DocumentCreatedAtCareUnit.Assigner}"",
                ""{_prefix}/dokument_skapat_på_vårdenhet_id|type"": ""{DocumentCreatedAtCareUnit.Type}"",
                ""{_prefix}/dokument_skapat_på_vårdenhet_namn"": ""{DocumentCreatedAtCareUnit.Name}"",";
            }

            if (DocumentSavedAtCareUnit != null)
            {
               result += $@"
                ""{_prefix}/dokument_sparat_på_vårdenhet_id"": ""{DocumentSavedAtCareUnit.Id}"",
                ""{_prefix}/dokument_sparat_på_vårdenhet_id|issuer"": ""{DocumentSavedAtCareUnit.Issuer}"",
                ""{_prefix}/dokument_sparat_på_vårdenhet_id|assigner"": ""{DocumentSavedAtCareUnit.Assigner}"",
                ""{_prefix}/dokument_sparat_på_vårdenhet_id|type"": ""{DocumentSavedAtCareUnit.Type}"",
                ""{_prefix}/dokument_sparat_på_vårdenhet_namn"": ""{DocumentSavedAtCareUnit.Name}"",";
            }

            return result;

        }
    }

}
