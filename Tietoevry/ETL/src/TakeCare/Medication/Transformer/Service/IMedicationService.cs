using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Service
{
    public interface IMedicationService 
    {
        EquivalenceModel GetEquivalenceDetails(string isReplaceble);
    }
}
