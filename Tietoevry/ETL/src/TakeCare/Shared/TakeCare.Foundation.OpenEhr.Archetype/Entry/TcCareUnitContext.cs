namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcCareUnitContext
    {
        private readonly string _prefix;
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
        public string CareUnitTerminology { get; set; } = "http://snomed.info/sct/900000000000207008";
        public string CareProviderTerminology { get; set; } = "http://snomed.info/sct/45991000052106";

        public TcCareUnitContext(string prefix)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            _prefix = $@"{prefix}/context/vårdenhet";
        }
        public override string ToString()
        {
            var result = $@"
                            ""{_prefix}/namn"": ""{CareUnitName}"",
                            ""{_prefix}/identifierare:0"": ""{CareUnitId}"",
                            ""{_prefix}/identifierare:0|issuer"": ""{Issuer}"",
                            ""{_prefix}/identifierare:0|assigner"": ""{Assigner}"",
                            ""{_prefix}/identifierare:0|type"": ""{CareUnitId}"",
                            ""{_prefix}/roll:0|code"": ""{Code}"",
                            ""{_prefix}/roll:0|value"": ""{Value}"",
                            ""{_prefix}/roll:0|terminology"": ""{CareUnitTerminology}"",
                            ""{_prefix}/vårdgivare/namn"": ""{CareUnitName}"",
                            ""{_prefix}/vårdgivare/identifierare:0"": ""{CareUnitId}"",
                            ""{_prefix}/vårdgivare/identifierare:0|issuer"": ""{Issuer}"",
                            ""{_prefix}/vårdgivare/identifierare:0|assigner"": ""{Assigner}"",
                            ""{_prefix}/vårdgivare/identifierare:0|type"": ""{CareProviderId}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0"": ""{OrgId}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|issuer"": ""{Issuer}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|assigner"": ""{Assigner}"",
                            ""{_prefix}/vårdgivare/organisationsnummer:0|type"": ""{CareProviderId}"",
                            ""{_prefix}/vårdgivare/roll:0|code"": ""{CareGiverCode}"",
                            ""{_prefix}/vårdgivare/roll:0|value"": ""{CareGiverValue}"",
                            ""{_prefix}/vårdgivare/roll:0|terminology"": ""{CareProviderTerminology}"",";
            return result;
        }

    }
}