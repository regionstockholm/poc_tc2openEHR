using TakeCare.Migration.OpenEhr.Application.Models;
using TakeCare.Migration.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public class TerminologyProvider :  ITerminologyProvider
    {
        private static List<TerminologyDetails> _terms { get; set; }
        static TerminologyProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "TcTerminology.json");
            _terms = Utility.LoadData<List<TerminologyDetails>>(filePath);
        }
        public TerminologyDetails GetTerminology(string termId)
        {
            return _terms.Find(t => t.TermId == termId);
        }
    }
}
