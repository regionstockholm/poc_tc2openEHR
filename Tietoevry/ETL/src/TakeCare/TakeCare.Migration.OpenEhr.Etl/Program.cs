using Spine.Migration.OpenEhr.Etl;
using TakeCare.Migration.OpenEhr.Etl;

Activation.Instance
               .Configure(args)
               .RegisterServices()
               .RegisterEtl<TakeCareEtlHandler>()
               .Build()
               .Run();

// Multiple ETL handlers of customers 