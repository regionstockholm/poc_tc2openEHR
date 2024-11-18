namespace TakeCare.Foundation.OpenEhr.Application
{
    using Services;
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IUnitProvider, UnitProvider>();
            register.AddSingleton<IPatientService, PatientService>();
            register.AddSingleton<ITerminologyProvider, TerminologyProvider>();
            register.AddSingleton<IContextProvider, ContextProvider>();
            register.AddSingleton<IRoleProvider, RoleProvider>();
            register.AddSingleton<IUserContextProvider, UserContextProvider>();
            register.AddSingleton<IFormProvider, FormProvider>();
            register.AddSingleton<IResultStatusService, ResultStatusService>();
        }
    }
}
