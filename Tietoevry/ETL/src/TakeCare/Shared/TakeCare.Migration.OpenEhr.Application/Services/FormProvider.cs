using Spine.Foundation.Web.OpenEhr.Client;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public class FormProvider : IFormProvider
    {
        private readonly IOpenEhrServiceAgent _openEhrAgent;

        public FormProvider(IOpenEhrServiceAgent openEhrAgent)
        {
            _openEhrAgent = openEhrAgent;
        }
    
        public async Task<Form> GetFormDetails(string formName)
        {
            Form formData =  await _openEhrAgent.GetFormDetails(formName);
            return (formData!=null) ? formData : 
                    new Form
                    {
                        Name = formName,
                        Version = "1.0.0"
                    };
        }

    }
}
