using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public interface IContextProvider
    {
        public ContextDetails GetContextData(string unitID);
    }
}
