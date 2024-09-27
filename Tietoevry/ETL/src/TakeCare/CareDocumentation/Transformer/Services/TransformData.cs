using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class TransformData : ITransformService
    {
        private readonly ICompositionService _compositionService;

        public TransformData(ICompositionService compositionService)
        {
            _compositionService = compositionService;
        }
        public CareDocumentOpenEhrData Transform(CareDocumentationDto inputData)
        {
            return _compositionService.Compose(inputData);
        }
    }
}