namespace TakeCare.Migration.OpenEhr.Measurement.Transformer
{

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IMeasurementTransformer, MeasurementTransformer>();
        }
    }
}
