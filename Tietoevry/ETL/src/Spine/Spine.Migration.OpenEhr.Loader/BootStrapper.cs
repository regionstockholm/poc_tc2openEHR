namespace Spine.Migration.OpenEhr.Loader
{
    public class BootStrapper : IBootStrapper
    {
        public void Register(IServiceRegister register)
        {
            register.AddSingleton<IOpenEhrLoader, OpenEhrLoader>();
        }
    }
}
