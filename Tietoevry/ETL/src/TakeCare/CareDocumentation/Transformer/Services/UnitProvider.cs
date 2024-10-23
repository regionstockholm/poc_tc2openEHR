using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class UnitProvider : IUnitProvider
    {
        private static List<UnitDetails> _units { get; set; }

        static UnitProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Units.json");
            _units = Utility.LoadData<List<UnitDetails>>(filePath);
        }

        public string GetOpenEhrUnit(string unit)
        {
            return _units.Find(u=>u.TakeCareUnit == unit)?.OpenEhrUnit ?? unit;
        }
    }
}
