namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction
{

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<ICareDocumentationExtractor, CareDocumentationExtractor>();
        }
    }
}
