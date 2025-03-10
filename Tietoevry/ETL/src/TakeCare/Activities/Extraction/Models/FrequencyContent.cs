using Newtonsoft.Json;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class FrequencyContent
    {
        public int? PlannedDate { get; set; }
        public int? PlannedTime { get; set; }
        public string? PlannedDateTime { get; set; }

        public int? StartDate { get; set; }
        public int? StartTime { get; set; }
        public string? StartDateTime { get; set; }


        public int? EndDate { get; set; }
        public int? EndTime { get; set; }
        public string? EndDateTime { get; set; }

        public int EveryNDays { get; set; }

        public int EveryNWeeks { get; set; }

        public List<int>? Times { get; set; }

        public List<string>? IrregularDateTimes { get; set; }

        public List<IntegerTime>? ConvertedTimes { get; set; }
    }

    public class IntegerTime
    {
        [JsonProperty("times")]
        public int Time { get; set; }
        public string? FormattedTime { get; set; }
    }
}
