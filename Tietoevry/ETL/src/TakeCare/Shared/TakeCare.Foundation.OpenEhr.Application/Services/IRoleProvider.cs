using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public interface IRoleProvider
    {
        RoleDetails GetProfessionName(string roleId);
    }
}
