using Spine.Foundation.Web.OpenEhr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    internal class FormProvider : IFormProvider
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
                        Name = "RSK - Journal Encounter Form",
                        Version = "1.0.0"
                    };
        }

    }
}
