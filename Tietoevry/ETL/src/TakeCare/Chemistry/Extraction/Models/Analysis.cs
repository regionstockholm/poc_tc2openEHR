using Newtonsoft.Json;
using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.Application.Utils;
namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class Analysis
    {
        [JsonProperty("analysis")]
        public string AnalysisInfo { get; set; }

        private string? analysisComment;

        public string? AnalysisComment
        {
            get => analysisComment;
            set => analysisComment = value?.Replace("\n", "\\n").Replace("\r", "\\n");
        }
        //public string AnalysisComment { get; set; }
        public string AnalysisId { get; set; }
        public string ReferenceArea { get; set; }
        public string Unit { get; set; }

        private string dataValue;

        public string Value
        {
            get
            {
                return dataValue;
            }
            set
            {
                dataValue = value?.GetNumberValue();
            }
        }



        public bool IsDeviating { get; set; }
    }
}
