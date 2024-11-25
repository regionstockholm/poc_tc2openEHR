namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class TcCareUnitContext
    {
        private string _prefix;

        public string CareUnitName { get; set; } 
        public string CareUnitId { get; set; } 
        public string Issuer { get; set; } = "RSK";
        public string Assigner { get; set; } = "RSK";
        public string CareUnitCode { get; set; } = "43741000";
        public string CareUnitValue { get; set; } = "vårdenhet";
        public string CareUnitTerminology { get; set; }
        public string CareProviderId { get; set; } 
        public string CareProviderName { get; set; }
        public string CareProviderCode { get; set; } = "143591000052106";
        public string CareProviderValue { get; set; } = "vårdgivare";
        public string CareProviderTerminology { get; set; }

        public Identifier OrgNumber { get; set; }

        public TcCareUnitContext(string prefix)
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
                ""{_prefix}/roll:0|terminology"": ""{CareUnitTerminology}"",
                ""{_prefix}/vårdgivare/namn"": ""{CareProviderName}"",
                ""{_prefix}/vårdgivare/identifierare:0"": ""{CareProviderId}"",
                ""{_prefix}/vårdgivare/identifierare:0|issuer"": ""{Issuer}"",
                ""{_prefix}/vårdgivare/identifierare:0|assigner"": ""{Assigner}"",
                ""{_prefix}/vårdgivare/identifierare:0|type"": ""CareProviderId"",
                ""{_prefix}/vårdgivare/roll:0|code"": ""{CareProviderCode}"",
                ""{_prefix}/vårdgivare/roll:0|value"": ""{CareProviderValue}"",
                ""{_prefix}/vårdgivare/roll:0|terminology"": ""{CareProviderTerminology}"",
                ""{_prefix}/vårdgivare/organisationsnummer:0"": ""{OrgNumber.Value}"",
                ""{_prefix}/vårdgivare/organisationsnummer:0|issuer"": ""{Issuer}"",
                ""{_prefix}/vårdgivare/organisationsnummer:0|assigner"": ""{Assigner}"",
                ""{_prefix}/vårdgivare/organisationsnummer:0|type"": ""{OrgNumber.Type}"",";
                
            return result;
        }
    }
}
