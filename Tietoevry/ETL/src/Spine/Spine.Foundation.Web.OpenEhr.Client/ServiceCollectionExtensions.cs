using Microsoft.Extensions.Configuration;
using Spine.Foundation.Web.OpenEhr.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenEhrClient(this IServiceCollection services, IConfiguration configuration)
        {
            var endPointUrl = configuration["Endpoint:Url"];

            if (endPointUrl == null)
            {
                throw new ArgumentNullException("Endpoint:Url is not set in configurations");
            }

            var idpConfig = configuration.GetSection("Idp");
            services.Configure<IdpConfigurations>(idpConfig);
            services.AddHttpClient("openEHRClient", client => client.BaseAddress = new Uri(endPointUrl));
            services.AddSingleton<IOpenEhrServiceAgent, OpenEhrServiceAgent>();
            services.AddSingleton<ITokenService, TokenService>();
            return services;
        }
    }
}
