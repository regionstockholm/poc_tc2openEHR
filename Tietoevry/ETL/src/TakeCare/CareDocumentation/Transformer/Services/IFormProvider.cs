using Spine.Foundation.Web.OpenEhr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public interface IFormProvider
    {
        public Task<Form> GetFormDetails(string formName);
    }
}
