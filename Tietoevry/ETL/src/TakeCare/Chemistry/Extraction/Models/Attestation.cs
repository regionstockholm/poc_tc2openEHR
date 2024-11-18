using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class Attestation
    {
        public Attested Attested { get; set; }
        public string? CreatedDateTime { get; set; }
        public string ResponsibleAttesterName { get; set; }
        public CareUnit CareUnit { get; set; }
        public Document Document { get; set; }
        public Patient Patient { get; set; }
        public ResponsibleAttester ResponsibleAttester { get; set; }
    }

    public class Attested
    {
        public string? DateTime { get; set; }

        public UserData User { get; set; }
    }
}
