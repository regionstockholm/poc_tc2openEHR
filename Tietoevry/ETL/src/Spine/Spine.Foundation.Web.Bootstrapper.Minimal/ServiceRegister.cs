public class ServiceRegister : IServiceRegister
{
    private readonly IBootstrapperContext _bootstrapperContext;
    public ServiceRegister(IBootstrapperContext bootstrapperContext)
    {
        _bootstrapperContext = bootstrapperContext;
    }

    public void AddScoped<TInterface, TService>() where TInterface : class
        where TService : class
    {
        AddServiceDescription<TInterface, TService>(ServiceLife.Scoped);
    }

    public void AddSingleton<TInterface, TService>() where TInterface : class
        where TService : class
    {
        AddServiceDescription<TInterface, TService>(ServiceLife.Singleton);
    }

    public void AddTransient<TInterface, TService>()
        where TInterface : class
        where TService : class
    {
        AddServiceDescription<TInterface, TService>(ServiceLife.Singleton);
    }
    private void AddServiceDescription<TInterface, TService>(ServiceLife lifetime)
        where TInterface : class
        where TService : class
    {
        _bootstrapperContext.Services.Add(new ServiceDescription
        {
            Contract = typeof(TInterface),
            Implmentaion = typeof(TService),
            Lifetime = lifetime
        });
    }
}
