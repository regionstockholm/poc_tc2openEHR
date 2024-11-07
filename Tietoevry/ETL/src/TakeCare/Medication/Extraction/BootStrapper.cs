namespace TakeCare.Migration.OpenEhr.Medication.Extraction
{
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IMedicationExtractor, MedicationExtractor>();
        }
    }
}
