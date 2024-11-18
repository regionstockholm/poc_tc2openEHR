using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Foundation.OpenEhr.Application.Services
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
