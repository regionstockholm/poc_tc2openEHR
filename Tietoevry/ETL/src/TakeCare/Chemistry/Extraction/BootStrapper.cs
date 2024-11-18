using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Services;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction
{

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IChemistryExtractor, ChemistryExtractor>();
            register.AddSingleton<IExtractionService, ExtractionService>();
        }
    }
}
