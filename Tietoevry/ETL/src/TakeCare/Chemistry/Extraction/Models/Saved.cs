using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class Saved
    {
        [JsonProperty("dateTime")]
        public string? SavedTimestamp { get; set; }
        public CareUnit CareUnit { get; set; }
    }
}
