using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public interface IResultStatusService
    {
        ResultStatusDetails GetResult(bool? flag);
    }
}
