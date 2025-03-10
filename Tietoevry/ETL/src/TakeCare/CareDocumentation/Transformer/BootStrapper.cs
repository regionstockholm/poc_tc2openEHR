namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer
{
    using Services;
    using TakeCare.Migration.OpenEhr.Etl.CareDocumentation;

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<ICompositionService, CompositionService>();
            register.AddSingleton<ICareDocumentationTransformer, CareDocumentationTransformer>();
        }
    }
}
