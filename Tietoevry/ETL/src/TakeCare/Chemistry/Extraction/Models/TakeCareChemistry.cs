using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class TakeCareChemistry
    {
        public List<ChemistryReply> ChemistryData { get; set; } = new List<ChemistryReply>();
    }
}
