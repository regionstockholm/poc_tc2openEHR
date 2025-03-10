using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Measurement.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Measurement.Transformer.Model;
using TakeCare.Migration.OpenEhr.Measurement.Transformer.Service;

namespace TakeCare.Migration.OpenEhr.Measurement.Transformer
{
    public class MeasurementTransformer : IMeasurementTransformer
    {
        private ICompositionService _compositionService;
        public MeasurementTransformer(ICompositionService compositionService)
        {
            _compositionService = compositionService;
        }

        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            var result = new OpenEhrMeasurement();
            ArgumentNullException.ThrowIfNull(input);
            result = _compositionService.TransformMeasurementInputToEhr(input.Result as MeasurementDto);

            return Task.FromResult<TResult>(result as TResult);
        }

    }
}
