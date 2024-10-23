using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class ContextDetails
    {
        public string CareUnitId { get; set; }
        public string CareUnitName { get; set; }

        public string CareProviderId { get; set; }
        public string CareProviderName { get; set; }

    }
}
