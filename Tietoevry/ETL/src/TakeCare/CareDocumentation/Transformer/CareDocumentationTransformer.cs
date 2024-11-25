using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.Etl.CareDocumentation
{
    public class CareDocumentationTransformer : ICareDocumentationTransformer
    {
        private readonly ICompositionService _compositionService;

        public CareDocumentationTransformer(ICompositionService compositionService)
        {
            _compositionService = compositionService;
        }
        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(input);

            var transformedData = _compositionService.Compose(input.Result as CareDocumentationDto);

            return Task.FromResult<TResult>(transformedData as TResult);
        }
    }
}
