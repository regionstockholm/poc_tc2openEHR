namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer
{
    using TakeCare.Migration.OpenEhr.Chemistry.Transformation;
    using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Services;

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IChemistryTransformer, ChemistryTransformer>();
            register.AddSingleton<ITransformService, TransformService>();
            register.AddSingleton<ICompositionService, CompositionService>();
        }
    }
}
