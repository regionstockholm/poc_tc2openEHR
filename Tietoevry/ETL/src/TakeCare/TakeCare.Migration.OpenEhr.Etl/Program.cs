using Spine.Migration.OpenEhr.Etl;
using TakeCare.Migration.OpenEhr.Etl.Handlers;

Activation.Instance
               .Configure(args)
               .Build()
               .Run<MedicationEtlHandler>();
              //.Run(); // Run all ETL handlers
                      //.Run<MeasurementEtlHandler>(); // Run individual ETL handler which you want to run


