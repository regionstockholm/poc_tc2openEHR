using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class Lab
    {
        public string SID { get; set; }
        public CareUnit CareUnit { get; set; }
    }
}
