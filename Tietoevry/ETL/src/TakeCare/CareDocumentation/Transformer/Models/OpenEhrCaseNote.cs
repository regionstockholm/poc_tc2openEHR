using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    public class OpenEhrCaseNote : BaseOpenEhrData
    {
        public string PatientID { get; set; }

        public TcContextInformation ContextInformation { get; set; }
        public TcCaseNoteCareUnitContext CareUnitContext { get; set; }
        public TcCaseNoteContextMetadata ContextMetadata { get; set; }

        public List<BaseEntry> Entries = new List<BaseEntry>();
        
        public override string ToString()
        {
            var result = $@"
                            {ContextInformation.ToString()}
                            {CareUnitContext.ToString()}
                            {ContextMetadata.ToString()}";

            foreach (var entry in Entries)
            {
                result += $@"{entry.ToString()}";
            }

            return "{" + result.TrimEnd().TrimEnd(',') + "}";
        }
    }
}
