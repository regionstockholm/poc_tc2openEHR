using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class TerminologyDetails
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string? Terminology { get; set; }
    }
}
