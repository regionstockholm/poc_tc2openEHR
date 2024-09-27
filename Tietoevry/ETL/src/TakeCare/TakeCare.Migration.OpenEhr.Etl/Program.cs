using Microsoft.Extensions.DependencyInjection;
using Spine.Migration.OpenEhr.Etl;
using TakeCare.Migration.OpenEhr.Etl;

Activation.Instance
               .Configure(args)
               .RegisterServices()
               .ConfigureServics(services =>
               {
                   services.AddHttpClient();
               })
               .RegisterEtl<TakeCareEtlHandler>()
               .Build()
               .Run();

// Multiple ETL handlers of customers 