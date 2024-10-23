using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class RoleProvider : IRoleProvider
    {
        private static List<RoleDetails> _roles { get; set; }
        static RoleProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "RoleData.json");
            _roles = Utility.LoadData<List<RoleDetails>>(filePath);
        }
        public RoleDetails GetProfessionName(string roleId)
        {
            return _roles.Find(t => t.Code == roleId);
        }
    }
}
