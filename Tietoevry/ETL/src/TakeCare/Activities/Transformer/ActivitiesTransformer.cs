using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Activities.Extraction.Models;
using TakeCare.Migration.OpenEhr.Activities.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer
{
    internal class ActivitiesTransformer : IActivitiesTransformer
    {
        private readonly ICompositionService _compositionService;

        public ActivitiesTransformer(ICompositionService compositionService)
        {
            _compositionService = compositionService;
        }
        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(input);

            var transformedData = _compositionService.Compose(input.Result as TakeCareActivities);

            return Task.FromResult<TResult>(transformedData as TResult);
        }
    }
}
