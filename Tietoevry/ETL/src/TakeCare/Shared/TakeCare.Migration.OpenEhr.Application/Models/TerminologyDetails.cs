using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Application.Models
{
    public class TerminologyDetails
    {
        public string TermId { get; set; }
        public string TermName { get; set; }
        public string Terminology { get; set; }
        public string Datatype { get; set; }
        public string Unit { get; set; }
    }
}
