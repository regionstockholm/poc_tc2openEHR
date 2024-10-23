namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcCareUnitContext
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

        public override string ToString()
        {
            var result = $@"
                            ""vårdkontakt/context/vårdenhet/namn"": ""{CareUnitName}"",
                            ""vårdkontakt/context/vårdenhet/identifierare:0"": ""{CareUnitId}"",
                            ""vårdkontakt/context/vårdenhet/identifierare:0|issuer"": ""{Issuer}"",
                            ""vårdkontakt/context/vårdenhet/identifierare:0|assigner"": ""{Assigner}"",
                            ""vårdkontakt/context/vårdenhet/identifierare:0|type"": ""{CareUnitId}"",
                            ""vårdkontakt/context/vårdenhet/roll:0|code"": ""{Code}"",
                            ""vårdkontakt/context/vårdenhet/roll:0|value"": ""{Value}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/namn"": ""{CareUnitName}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/identifierare:0"": ""{CareUnitId}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/identifierare:0|issuer"": ""{Issuer}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/identifierare:0|assigner"": ""{Assigner}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/identifierare:0|type"": ""{CareProviderId}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/organisationsnummer:0"": ""{OrgId}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/organisationsnummer:0|issuer"": ""{Issuer}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/organisationsnummer:0|assigner"": ""{Assigner}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/organisationsnummer:0|type"": ""{CareProviderId}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/roll:0|code"": ""{CareGiverCode}"",
                            ""vårdkontakt/context/vårdenhet/vårdgivare/roll:0|value"": ""{CareGiverValue}"",";
            return result;
        }

    }
}