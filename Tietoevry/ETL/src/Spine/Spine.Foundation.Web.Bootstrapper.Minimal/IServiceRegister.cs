
public interface IServiceRegister
{
    void AddSingleton<TInterface, TService>() where TInterface : class where TService : class;
    void AddScoped<TInterface, TService>() where TInterface : class where TService : class;
    void AddTransient<TInterface, TService>() where TInterface : class where TService : class;
}