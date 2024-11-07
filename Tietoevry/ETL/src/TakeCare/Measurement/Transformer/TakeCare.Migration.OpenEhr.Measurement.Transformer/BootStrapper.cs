using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;
using TakeCare.Migration.OpenEhr.Etl.CareDocumentation;
namespace TakeCare.Migration.OpenEhr.Measurement.Transformer
{

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IUnitProvider, UnitProvider>();
            register.AddSingleton<IPatientService, PatientService>();
            register.AddSingleton<ITerminologyProvider, TerminologyProvider>();
            register.AddSingleton<IRoleProvider, RoleProvider>();
            register.AddSingleton<IMeasurementTransformer, MeasurementTransformer>();
        }
    }
}
