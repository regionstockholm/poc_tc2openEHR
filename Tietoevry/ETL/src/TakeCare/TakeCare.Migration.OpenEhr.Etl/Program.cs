using Spine.Migration.OpenEhr.Etl;
Activation.Instance
               .Configure(args)
               .Build()
               .Run(); // Run all ETL handlers
               //.Run<MeasurementEtlHandler>(); // Run individual ETL handler which you want to run


