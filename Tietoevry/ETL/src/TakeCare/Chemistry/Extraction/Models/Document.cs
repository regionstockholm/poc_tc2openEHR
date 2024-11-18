using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class Document
    {
        public bool? HasDeviatingAnalysis { get; set; }
        public string Id { get; set; }
        public string? SavedDateTime { get; set; }
        public bool IsLatestVersionAttested { get; set; }
        public Type Type { get; set; }
    }
}
