using TakeCare.Migration.OpenEhr.Measurement.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Measurement.Transformer.Model;

namespace TakeCare.Migration.OpenEhr.Measurement.Transformer.Service
{
    public interface ICompositionService
    {
        OpenEhrMeasurement TransformMeasurementInputToEhr(MeasurementDto? measurementDto);
    }
}
