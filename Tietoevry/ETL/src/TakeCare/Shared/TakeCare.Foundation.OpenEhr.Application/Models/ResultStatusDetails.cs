using Newtonsoft.Json;

namespace TakeCare.Foundation.OpenEhr.Application.Models
{
    public class ResultStatusDetails
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }

        [JsonProperty("target")]
        public List<Target> Target { get; set; }

    }

    public class Target
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }

        [JsonProperty("equivalence")]
        public string Equivalence { get; set; }
    }

}
