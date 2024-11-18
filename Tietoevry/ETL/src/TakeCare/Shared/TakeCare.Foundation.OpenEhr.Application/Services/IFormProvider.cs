using Spine.Foundation.Web.OpenEhr.Client;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public interface IFormProvider
    {
        public Task<Form> GetFormDetails(string formName);
    }
}
