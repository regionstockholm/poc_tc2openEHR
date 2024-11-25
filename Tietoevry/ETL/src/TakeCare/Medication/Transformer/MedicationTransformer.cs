using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Medication.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Service;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer
{
    public class MedicationTransformer : IMedicationTransformer
    {
        private readonly ICompositionService _compositionService;
        public MedicationTransformer(ICompositionService compositionService)
        {
            _compositionService = compositionService;
        }
        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            var result = new List<OpenEhrMedication>();
            ArgumentNullException.ThrowIfNull(input);
            result = _compositionService.TransformMeasurementInputToEhr(input.Result as MedicationDTO);

            return Task.FromResult<TResult>(result as TResult);
        }

    }
}
