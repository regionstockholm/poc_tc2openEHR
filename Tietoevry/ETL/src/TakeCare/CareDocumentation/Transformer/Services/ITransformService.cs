using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public interface ITransformService
    {
        CareDocumentOpenEhrData Transform(CareDocumentationDto inputData);
    }
}