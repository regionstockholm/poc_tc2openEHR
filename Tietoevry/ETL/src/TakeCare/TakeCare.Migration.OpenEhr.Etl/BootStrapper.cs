namespace TakeCare.Migration.OpenEhr.Etl
{
    using Handlers;
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<ICareDocumentationEtlHandler, CareDocumentationEtlHandler>();
        }
    }
}
