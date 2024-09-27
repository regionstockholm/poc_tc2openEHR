namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer
{
    using Services;
    using TakeCare.Migration.OpenEhr.Etl.CareDocumentation;

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<ITerminolgyLookup, TerminologyLookup>();
            register.AddSingleton<ITemplateServices, TemplateServices>();
            register.AddSingleton<ICompositionService, CompositionService>();

            register.AddSingleton<ITransformService, TransformData>();

            register.AddSingleton<ICareDocumentationTransformer, CareDocumentationTransformer>();
        }
    }
}
