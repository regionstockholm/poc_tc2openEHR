using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Service;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer
{
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IUnitProvider, UnitProvider>();
            register.AddSingleton<IPatientService, PatientService>();
            register.AddSingleton<IMedicationService, MedicationService>();
            register.AddSingleton<ITerminologyProvider, TerminologyProvider>();
            register.AddSingleton<IContextProvider, ContextProvider>();
            register.AddSingleton<IMedicationTransformer, MedicationTransformer>();
        }
    }
}
