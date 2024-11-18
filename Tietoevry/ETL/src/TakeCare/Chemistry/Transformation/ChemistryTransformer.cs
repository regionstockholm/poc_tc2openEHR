using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Services;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation
{
    public class ChemistryTransformer : IChemistryTransformer
    {
        private readonly ITransformService _transformService;

        public ChemistryTransformer(ITransformService transformService)
        {
            _transformService = transformService;
        }
        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            ArgumentNullException.ThrowIfNull(input);

            var transformedData = _transformService.Transform(input.Result as TakeCareChemistry);

            return Task.FromResult<TResult>(transformedData as TResult);
        }
    }
}
    