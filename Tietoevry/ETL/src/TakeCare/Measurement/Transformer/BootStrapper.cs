using TakeCare.Migration.OpenEhr.Measurement.Transformer.Service;

namespace TakeCare.Migration.OpenEhr.Measurement.Transformer
{

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<ICompositionService, CompositionService>();
            register.AddSingleton<IMeasurementTransformer, MeasurementTransformer>();
        }
    }
}
