using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class CareDocConfig
    {
        public EhrConfig Ehr { get; set; }

        public Template Template { get; set; }

        public FormConfig Form { get; set; }

        public Language Language { get; set; }
    }
}
