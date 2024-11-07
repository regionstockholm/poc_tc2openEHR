namespace TakeCare.Migration.OpenEhr.Measurement.Extraction
{
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IMeasurementExtractor, MeasurementExtractor>();
        }
    }
}
