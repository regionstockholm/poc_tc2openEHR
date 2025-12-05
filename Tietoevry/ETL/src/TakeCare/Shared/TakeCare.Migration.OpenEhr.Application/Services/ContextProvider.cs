using TakeCare.Migration.OpenEhr.Application.Models;
using TakeCare.Migration.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public class ContextProvider : IContextProvider
    {
        private static List<ContextDetails> _roles { get; set; }
        static ContextProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Context.json");
            _roles = Utility.LoadData<List<ContextDetails>>(filePath);
        }
        public ContextDetails GetContextData(string unitID)
        {
            return _roles.Find(t => t.TakeCareUnitId.Equals(unitID));
        }
    }
}
