using System.Text.Json;
using System.Xml.Linq;
using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class TcContextReportID
    {
        public string _prefix { get; set; }
        public string ReportID { get; set; }
        public string Synopsis { get; set; }
        public LabResult LabResult { get; set; }

        public CodedText Language { get; set; }
        public CodedText Territory { get; set; }
        public CodedText Category { get; set; }


        public TcContextReportID(string prefix)
        {
            _prefix = prefix;
        }

        public override string ToString()
        {
            var result = $@"
                ""{_prefix}/context/rapport-id"": ""{ReportID}"",";

            if (!string.IsNullOrWhiteSpace(Synopsis))
            {
                result += $@"
                ""{_prefix}/klinisk_synopsis/synopsis"": ""{Synopsis}"",";
            }

            if (LabResult != null)
            {
                result += $@"
                ""{_prefix}/composer|name"": ""{LabResult.Name}"",";

                if (LabResult.Context != null)
                {
                    result += $@"
                ""{_prefix}/context/start_time"": ""{LabResult.Context.StartTime}"",";

                    if (LabResult.Context.HealthCareFacility != null)
                    {
                        result += $@"
                ""{_prefix}/context/_health_care_facility|name"": ""{LabResult.Context.HealthCareFacility.Name}"",";

                        result += $@"
                ""{_prefix}/context/_health_care_facility/_identifier:0"": ""{LabResult.Context.HealthCareFacility.Identifiers.FirstOrDefault()}"",";

                        result += $@"
                ""{_prefix}/context/_health_care_facility/_identifier:0|type"": ""{CompositionConstants.CARE_UNIT_HSA_ID_OID_MARKER}"",";

                    }
                }
            }

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
    public class LabResult
    {
        public string Name { get; set; }
        public Context Context { get; set; }
    }

    public class Context
    {
        public HealthCareFacility HealthCareFacility { get; set; }
        public string StartTime { get; set; }
    }

    public class HealthCareFacility
    {
        public string Name { get; set; }
        public List<string> Identifiers { get; set; }
    }


}
