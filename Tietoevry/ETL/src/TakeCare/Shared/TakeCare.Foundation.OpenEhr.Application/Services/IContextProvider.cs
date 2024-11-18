using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public interface IContextProvider
    {
        public ContextDetails GetContextData(string unitID);
    }
}
