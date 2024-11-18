using TakeCare.Migration.OpenEhr.Medication.Transformer.Service;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer
{
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IMedicationService, MedicationService>();
            register.AddSingleton<IMedicationTransformer, MedicationTransformer>();
        }
    }
}
