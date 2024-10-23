using Spine.Migration.OpenEhr.Etl.Core;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using Spine.Migration.OpenEhr.Loader;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.Measurement.Extraction;
using TakeCare.Migration.OpenEhr.Measurement.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Measurement.Transformer;
using TakeCare.Migration.OpenEhr.Measurement.Transformer.Model;

namespace TakeCare.Migration.OpenEhr.Etl.Handlers
{
    public class MeasurementEtlHandler : IEtlHandler
    {
        private readonly Lazy<IMeasurementExtractor> _lazyExtractor;
        private readonly Lazy<IMeasurementTransformer> _lazyTransformer;
        private readonly Lazy<IOpenEhrLoader> _lazyLoader;

        private IMeasurementExtractor _measurementExtractor => _lazyExtractor.Value;
        private IMeasurementTransformer _measurementTransformer => _lazyTransformer.Value;
        private IOpenEhrLoader _OpenEhrLoader => _lazyLoader.Value;

        public MeasurementEtlHandler(Lazy<IMeasurementExtractor> lazyExtractor,
            Lazy<IMeasurementTransformer> lazyTransformer,
            Lazy<IOpenEhrLoader> lazyLoader)
        {
            _lazyExtractor = lazyExtractor;
            _lazyTransformer = lazyTransformer;
            _lazyLoader = lazyLoader;
        }


        public async Task Execute()
        {
            int count = 0;
            string careDocsFolder = Path.Combine(AppContext.BaseDirectory, @"Assets\TestData\Measurement");
            var files = Directory.EnumerateFiles(careDocsFolder, "*.json");
            foreach (var file in files)
            {
                Console.WriteLine($"{++count}. File name : {file}\n");
                try
                {
                    var extractorConfigurations = new ExtractionConfiguration<string>(file);
                    var measurementData = await  _measurementExtractor.Extract<string, MeasurementDto>(extractorConfigurations);

                    Console.WriteLine($"*Measurements for Patient - {measurementData.Result.PatientId}");

                    //todo - transform is in progress
                    var tcOpenEhrData =  await _measurementTransformer.Trasform<MeasurementDto, OpenEhrMeasurement>(measurementData);

                    //todo - load

                    var result = await _OpenEhrLoader.Load<OpenEhrData<OpenEhrMeasurement>, object>(new OpenEhrData<OpenEhrMeasurement>()
                    {
                        PatientID = tcOpenEhrData.PatientID,
                        Compositions = new List<OpenEhrMeasurement>() { tcOpenEhrData }
                        //Compositions = new List<JObject>() { JObject.Parse("{" +tcOpenEhrData.ToString() + "}") }
                    }); 
                }
                catch (Exception ex)
                {
                    // ToDo Log exceptions in Logger
                    Console.WriteLine(ex.ToString());

                }
            }
        }

      
    }
}
