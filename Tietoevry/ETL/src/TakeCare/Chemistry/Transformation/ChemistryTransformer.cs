using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Services;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation
{
    public class ChemistryTransformer : IChemistryTransformer
    {
        private readonly ICompositionService _compositionService;

        public ChemistryTransformer(ICompositionService compositionService)
        {
            _compositionService = compositionService;
        }
        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(input);

            var transformedData = _compositionService.Compose(input.Result as TakeCareChemistry);

            return Task.FromResult<TResult>(transformedData as TResult);
        }
    }
}
    