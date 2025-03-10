using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class CareUnitDetails
    {
        public string Id { get; set; }
        public string Issuer { get; set; }
        public string Assigner { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
