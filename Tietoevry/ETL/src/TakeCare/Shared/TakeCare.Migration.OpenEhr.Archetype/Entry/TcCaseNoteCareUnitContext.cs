namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    public class TcCaseNoteCareUnitContext
    {
        private string _prefix;
        public string CareUnitName { get; set; }
        public Identifier CareUnitId { get; set; }
        public string CareUnitCode { get; set; } = "43741000";
        public string CareUnitValue { get; set; } = "vårdenhet";
        public string CareUnitTerminology { get; set; }
        public Identifier CareProviderId { get; set; }
        public string CareProviderName { get; set; }
        public string CareProviderCode { get; set; } = "143591000052106";
        public string CareProviderValue { get; set; } = "vårdgivare";
        public string CareProviderTerminology { get; set; }
        public Identifier OrgId { get; set; }

        public TcCaseNoteCareUnitContext(string prefix)
        {
            _prefix = $@"{prefix}/context/vårdenhet";
        }
        public override string ToString()
        {
            var result = $@"
                            ""{_prefix}/namn"": ""{CareUnitName}"",
                            ""{_prefix}/identifierare:0"": ""{CareUnitId.Value}"",
                            ""{_prefix}/identifierare:0|issuer"": ""{CareUnitId.Issuer}"",
                            ""{_prefix}/identifierare:0|assigner"": ""{CareUnitId.Assigner}"",
                            ""{_prefix}/identifierare:0|type"": ""{CareUnitId.Type}"",
                            ""{_prefix}/roll:0|code"": ""{CareUnitCode}"",
                            ""{_prefix}/roll:0|value"": ""{CareUnitValue}"",
                            ""{_prefix}/roll:0|terminology"": ""{CareUnitTerminology}"",
                            ""{_prefix}/vårdgivare/namn"": ""{CareProviderName}"",
                            ""{_prefix}/vårdgivare/identifierare:0"": ""{CareProviderId.Value}"",
                            ""{_prefix}/vårdgivare/identifierare:0|issuer"": ""{CareProviderId.Issuer}"",
                            ""{_prefix}/vårdgivare/identifierare:0|assigner"": ""{CareProviderId.Assigner}"",
                            ""{_prefix}/vårdgivare/identifierare:0|type"": ""{CareProviderId.Type}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|id"": ""{OrgId.Value}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|type"": ""{OrgId.Type}"",
                            ""{_prefix}/vårdgivare/roll:0|code"": ""{CareProviderCode}"",
                            ""{_prefix}/vårdgivare/roll:0|value"": ""{CareProviderValue}"",
                            ""{_prefix}/vårdgivare/roll:0|terminology"": ""{CareProviderTerminology}"",";

            
            //""{_prefix}/vårdgivare/organisationsnummer:0|issuer"": ""{OrgId.Issuer}"",
            //""{_prefix}/vårdgivare/organisationsnummer:0|assigner"": ""{OrgId.Assigner}"",
            //""{_prefix}/vårdgivare/organisationsnummer:0|type"": ""{OrgId.Type}"",
            return result;
        }

    }
}