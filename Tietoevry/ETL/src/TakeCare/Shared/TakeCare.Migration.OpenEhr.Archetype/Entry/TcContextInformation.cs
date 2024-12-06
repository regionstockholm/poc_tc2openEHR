namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    public class TcContextInformation
    {
        public string _prefix { get; set; }
        public ComposerIdentifier Composer { get; set; }
        public string StartTime { get; set; }
        public Setting Setting { get; set; }
        public HealthCareFacilityIdentifier HealthCareFacility { get; set; }

        public CodedText Language { get; set; }
        public CodedText Territory { get; set; }
        public CodedText Category { get; set; }

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
                            ""{_prefix}/context/_health_care_facility|name"": ""Creating Care Unit"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|name"": ""{HealthCareFacility.Name}"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|id"": ""{HealthCareFacility.Id}"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|type"": ""{HealthCareFacility.Type}"",
                            ""{_prefix}/context/_health_care_facility/_identifier:0|issuer"": ""{HealthCareFacility.Issuer}"",";

            if (Language != null)
            {
                result += $@"
                ""{_prefix}/language|code"": ""{Language.Code}"",
                ""{_prefix}/language|terminology"": ""{Language.Terminology}"",";
            }
            if (Category != null)
            {
                result += $@"
                ""{_prefix}/category|code"": ""{Category.Code}"",
                ""{_prefix}/category|value"": ""{Category.Value}"",
                ""{_prefix}/category|terminology"": ""{Category.Terminology}"",";
            }
            if (Territory != null)
            {
                result += $@"
                ""{_prefix}/territory|code"": ""{Territory.Code}"",
                ""{_prefix}/territory|terminology"": ""{Territory.Terminology}"",";
            }

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
