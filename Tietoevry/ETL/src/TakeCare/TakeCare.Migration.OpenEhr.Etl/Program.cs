﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Spine.Migration.OpenEhr.Etl;
using TakeCare.Migration.OpenEhr.Activities.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models;
using TakeCare.Migration.OpenEhr.Etl.Handlers;
using TakeCare.Migration.OpenEhr.Measurement.Transformer.Model;
using TakeCare.Migration.OpenEhr.Medication.Transformer.MapperProfile;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Service;

await Activation.Instance
               .Configure(args)
               .ConfigureServics((services, configuration) =>
               {
                   var chemistryConfig = configuration.GetSection("Chemistry");
                   services.Configure<ChemistryConfig>(chemistryConfig);

                   var careDocConfig = configuration.GetSection("CareDocumentation");
                   services.Configure<CareDocConfig>(careDocConfig);

                   var measurementConfig = configuration.GetSection("Measurement");
                   services.Configure<MeasurementConfig>(measurementConfig);

                   var medicationConfig = configuration.GetSection("Medication");
                   services.Configure<MedicationConfig>(medicationConfig);

                   var activityConfig = configuration.GetSection("Activity");
                   services.Configure<ActivitiesConfig>(activityConfig);

                   services.AddAutoMapper(config => {
                   var servicePro = services.BuildServiceProvider().CreateScope().ServiceProvider;
                   var medicationService = servicePro.GetService<IMedicationService>();
                   config.AddProfile(new MedicationMapperProfile(medicationService));
               });
               })
               .Build("TakeCare.Migration.OpenEhr")
//.Run<MedicationEtlHandler>();
.Run(); // Run all ETL handlers
//.Run<MeasurementEtlHandler>(); // Run individual ETL handler which you want to run

