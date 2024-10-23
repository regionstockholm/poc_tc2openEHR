namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcContextInformation
    {
        public string _prefix { get; set; }
        public ComposerIdentifier Composer { get; set; }
        public string StartTime { get; set; }
        public Setting Setting { get; set; }
        public HealthCareFacilityIdentifier HealthCareFacility { get; set; }

        public TcContextInformation(string prefix)
        {
            _prefix = prefix;
        }

        public override string ToString()
        {
            var result = $@"
                            ""{_prefix}/composer/_identifier:0|name"": ""{Composer.Name}"",
                            ""{_prefix}/composer/_identifier:0|id"" : ""{Composer.Id}"",
                            ""{_prefix}/composer/_identifier:0|type"": ""{Composer.Type}"",
                            ""{_prefix}/composer/_identifier:0|issuer"": ""{Composer.Issuer}"",
                            ""{_prefix}/context/start_time"": ""{StartTime}"",
                            ""{_prefix}/context/setting|code"": ""{Setting.Code}"",
                            ""{_prefix}/context/setting|value"": ""{Setting.Value}"",
                            ""{_prefix}/context/setting|terminology"": ""{Setting.Terminology}"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|name"": ""{HealthCareFacility.Name}"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|id"": ""{HealthCareFacility.Id}"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|type"": ""{HealthCareFacility.Type}"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|issuer"": ""{HealthCareFacility.Issuer}"",";

            return result;
        }
    }

    public class ComposerIdentifier
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Issuer { get; set; }
    }

    public class HealthCareFacilityIdentifier
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Issuer { get; set; }
    }
    public class Setting
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string Terminology { get; set; }
    }
}
