using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class TerminologyProvider :  ITerminologyProvider
    {
        private static List<TerminologyDetails> _terms { get; set; }
        public TerminologyProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Terminology.json");
            _terms = Utility.LoadData<List<TerminologyDetails>>(filePath);
        }
        public TerminologyDetails GetTerminology(string termId)
        {
            return _terms.Find(t => t.Code == termId);
        }
    }
}
