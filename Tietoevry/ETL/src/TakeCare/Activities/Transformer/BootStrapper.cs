namespace TakeCare.Migration.OpenEhr.Activities.Transformer
{
    using Services;
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<ICompositionService, CompositionService>();
            register.AddSingleton<IActivitiesTransformer, ActivitiesTransformer>();
        }
    }
}
