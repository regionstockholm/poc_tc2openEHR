using Microsoft.Extensions.Logging;
using Spine.Migration.OpenEhr.Etl.Core;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using Spine.Migration.OpenEhr.Loader;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.Medication.Extraction;
using TakeCare.Migration.OpenEhr.Medication.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Medication.Transformer;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;

namespace TakeCare.Migration.OpenEhr.Etl.Handlers
{
    public class MedicationEtlHandler //: IEtlHandler
    {
        private readonly Lazy<IMedicationExtractor> _lazyExtractor;
        private readonly Lazy<IMedicationTransformer> _lazyTransformer;
        private readonly Lazy<IOpenEhrLoader> _lazyLoader;

        private readonly Lazy<ILogger<MedicationEtlHandler>> _lazyLogger;
        private ILogger<MedicationEtlHandler> _logger => _lazyLogger.Value;
        private IMedicationExtractor _medicationExtractor => _lazyExtractor.Value;
        private IMedicationTransformer _medicationTransformer => _lazyTransformer.Value;
        private IOpenEhrLoader _OpenEhrLoader => _lazyLoader.Value;

        public MedicationEtlHandler(Lazy<IMedicationExtractor> lazyExtractor,
            Lazy<IMedicationTransformer> lazyTransformer,
            Lazy<IOpenEhrLoader> lazyLoader,
            Lazy<ILogger<MedicationEtlHandler>> lazyLogger)
        {
            _lazyExtractor = lazyExtractor;
            _lazyTransformer = lazyTransformer;
            _lazyLoader = lazyLoader;
            _lazyLogger = lazyLogger;
        }

        public async Task Execute()
        {
            int count = 0;
            string careDocsFolder = Path.Combine(AppContext.BaseDirectory, @"Assets\TestData\Medication");
            var files = Directory.EnumerateFiles(careDocsFolder, "*.xml");
            foreach (var file in files)
            {
                try
                {
                    var extractorConfigurations = new ExtractionConfiguration<string>(file);
                    var medicationData = await _medicationExtractor.Extract<string, MedicationDTO>(extractorConfigurations);
                    var tcOpenEhrData = await _medicationTransformer.Trasform<MedicationDTO, List<OpenEhrMedication>>(medicationData);

                    foreach (var medication in tcOpenEhrData)
                    {
                        var result = await _OpenEhrLoader.Load<OpenEhrData<OpenEhrMedication>, object>(new OpenEhrData<OpenEhrMedication>()
                        {
                            PatientID = medication.PatientID,
                            Compositions = new List<OpenEhrMedication>() { medication }
                        });

                        //    //need to write logic to call load separately for 
                        //    foreach (var openEhr in medication.Infusions)
                        //    {
                        //        Console.WriteLine("Infusion - inprocess");
                        //    }
                        //    foreach (var openEhr in medication.Administrations)
                        //    {
                        //        Console.WriteLine("Administration - inprocess");
                        //    }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in processing file {file}");
                }

                _logger.LogInformation($"{++count}. File name : {file}");
            }
        }


    }
}
