namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcCaseNoteCareUnitContext
    {
        private string _prefix;
        public string CareUnitName { get; set; } = "CareUnitName";
        public string CareUnitId { get; set; } = "CareUnitID";
        public string Issuer { get; set; } = "RSK";
        public string Assigner { get; set; } = "RSK";
        public string CareUnitCode { get; set; } = "43741000";
        public string CareUnitValue { get; set; } = "vårdenhet";
        public string CareProviderId { get; set; } = "CareProviderId";
        public string OrgId { get; set; } = "OrgId";
        public string CareProviderName { get; set; }
        public string CareProviderCode { get; set; } = "143591000052106";
        public string CareProviderValue { get; set; } = "vårdgivare";

        public TcCaseNoteCareUnitContext(string prefix)
        {
            _prefix = prefix;
        }
        public override string ToString()
        {
            var result = $@"
                            ""{_prefix}/namn"": ""{CareUnitName}"",
                            ""{_prefix}/identifierare:0"": ""{CareUnitId}"",
                            ""{_prefix}/identifierare:0|issuer"": ""{Issuer}"",
                            ""{_prefix}/identifierare:0|assigner"": ""{Assigner}"",
                            ""{_prefix}/identifierare:0|type"": ""CareUnitId"",
                            ""{_prefix}/roll:0|code"": ""{CareUnitCode}"",
                            ""{_prefix}/roll:0|value"": ""{CareUnitValue}"",
                            ""{_prefix}/vårdgivare/namn"": ""{CareProviderName}"",
                            ""{_prefix}/vårdgivare/identifierare:0"": ""{CareProviderId}"",
                            ""{_prefix}/vårdgivare/identifierare:0|issuer"": ""{Issuer}"",
                            ""{_prefix}/vårdgivare/identifierare:0|assigner"": ""{Assigner}"",
                            ""{_prefix}/vårdgivare/identifierare:0|type"": ""CareProviderId"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0"": ""{CareProviderId}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|issuer"": ""{Issuer}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|assigner"": ""{Assigner}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|type"": ""CareProviderId"",
                            ""{_prefix}/vårdgivare/roll:0|code"": ""{CareProviderCode}"",
                            ""{_prefix}/vårdgivare/roll:0|value"": ""{CareProviderValue}"",";
            return result;
        }

    }
}