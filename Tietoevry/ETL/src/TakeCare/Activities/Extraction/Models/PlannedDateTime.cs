using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class PlannedDateTime
    {
        public int? Date { get; set; }

        public int? Time { get; set; }

        public string? DateTime { get; set; } // combined datetime
    }
}
