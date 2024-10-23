namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer
{
    using Services;
    using TakeCare.Migration.OpenEhr.Etl.CareDocumentation;

    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<ICompositionService, CompositionService>();
            register.AddSingleton<IUnitProvider, UnitProvider>();
            register.AddSingleton<ITransformService, TransformData>();
            register.AddSingleton<IPatientService, PatientService>();
            register.AddSingleton<ITerminologyProvider, TerminologyProvider>();
            register.AddSingleton<IRoleProvider, RoleProvider>();
            register.AddSingleton<IContextProvider, ContextProvider>();
            register.AddSingleton<IUserContextProvider, UserContextProvider>();
            register.AddSingleton<IFormProvider, FormProvider>();
            register.AddSingleton<ICareDocumentationTransformer, CareDocumentationTransformer>();
        }
    }
}
