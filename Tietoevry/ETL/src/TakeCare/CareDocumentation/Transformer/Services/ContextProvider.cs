using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    internal class ContextProvider : IContextProvider
    {
        private static List<ContextDetails> _roles { get; set; }
        static ContextProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Context.json");
            _roles = Utility.LoadData<List<ContextDetails>>(filePath);
        }
        public ContextDetails GetContextData(string unitID)
        {
            return _roles.Find(t => t.CareUnitId.Equals(unitID));
        }
    }
}
