using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public interface IResultStatusService
    {
        ResultStatusDetails GetResult(bool? flag);
    }
}
