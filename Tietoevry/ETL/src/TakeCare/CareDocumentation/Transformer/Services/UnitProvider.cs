using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    internal class UnitProvider : IUnitProvider
    {
        private static Dictionary<string, string> dictionary { get; set; }

        static UnitProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Units.json");
            dictionary = Utility.LoadData<Dictionary<string, string>>(filePath);
        }

        public string GetOpenEhrUnit(string unit)
        {
            return dictionary.ContainsKey(unit) ? dictionary[unit] : unit;
        }
    }
}
