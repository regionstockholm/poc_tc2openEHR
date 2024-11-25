using TakeCare.Migration.OpenEhr.Activities.Extraction.Services;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction
{

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IActivitiesExtractor, ActivitiesExtractor>();
            register.AddSingleton<IExtractionService, ExtractionService>();
            register.AddSingleton<IFormatService, FormatService>();
        }
    }
}
