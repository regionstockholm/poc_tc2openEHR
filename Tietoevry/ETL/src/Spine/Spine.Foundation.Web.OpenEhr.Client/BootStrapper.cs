namespace Spine.Foundation.Web.OpenEhr.Client
{
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IOpenEhrServiceAgent, OpenEhrServiceAgent>();
            register.AddSingleton<ITokenService, TokenService>();
        }
    }
}
