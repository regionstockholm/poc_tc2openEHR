using TakeCare.Migration.OpenEhr.Medication.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Service
{
    public interface ICompositionService
    {
        List<OpenEhrMedication> TransformMeasurementInputToEhr(MedicationDTO? medicationDto);
    }
}
