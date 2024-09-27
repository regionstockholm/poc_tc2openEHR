
using Microsoft.Extensions.DependencyInjection;

public static class BootstrapServicesExtensions
{
    public static void AddBootStrapper(this IServiceCollection services, string searchPattern = "")
    {
        BootstrapperBuilder.Build(services, searchPattern);
    }
}
