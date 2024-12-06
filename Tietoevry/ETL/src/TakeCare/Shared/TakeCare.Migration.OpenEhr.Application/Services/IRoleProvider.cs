using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public interface IRoleProvider
    {
        RoleDetails GetProfessionName(string roleId);
    }
}
