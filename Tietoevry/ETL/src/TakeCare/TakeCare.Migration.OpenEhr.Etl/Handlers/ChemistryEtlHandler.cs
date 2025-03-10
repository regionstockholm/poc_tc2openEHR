using Spine.Migration.OpenEhr.Etl.Core;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using Spine.Migration.OpenEhr.Loader;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation;
using Microsoft.Extensions.Logging;

namespace TakeCare.Migration.OpenEhr.Etl.Handlers
{
    public class ChemistryEtlHandler : IEtlHandler
    {
        private readonly Lazy<IChemistryExtractor> _lazyExtractor;
        private readonly Lazy<IChemistryTransformer> _lazyTransformer;
        private readonly Lazy<IOpenEhrLoader> _lazyLoader;
        private readonly Lazy<ILogger<ChemistryEtlHandler>> _lazyLogger;
        private ILogger<ChemistryEtlHandler> _logger => _lazyLogger.Value;
        private IChemistryExtractor _chemistryDocExtractor => _lazyExtractor.Value;
        private IChemistryTransformer _chemistryTransformer => _lazyTransformer.Value;
        private IOpenEhrLoader _chemistryOpenEhrLoader => _lazyLoader.Value;

        public ChemistryEtlHandler(Lazy<IChemistryExtractor> lazyExtractor,
            Lazy<IChemistryTransformer> lazyTransformer, Lazy<IOpenEhrLoader> lazyLoader,
            Lazy<ILogger<ChemistryEtlHandler>> lazyLogger)
        {
            _lazyExtractor = lazyExtractor;
            _lazyTransformer = lazyTransformer;
            _lazyLoader = lazyLoader;
            _lazyLogger = lazyLogger;
        }


        public async Task Execute()
        {
            int count = 0;
            string chemistryFolder = Path.Combine(AppContext.BaseDirectory, @"Assets\TestData\Chemistry");
            foreach (var file in Directory.EnumerateFiles(chemistryFolder, "*.json"))
            {
                try
                {
                    var extractorConfigurations = new ExtractionConfiguration<string>(file);
                    var tcData = await _chemistryDocExtractor.Extract<string, TakeCareChemistry>(extractorConfigurations);
                    var tcOpenEhrData = await _chemistryTransformer.Trasform<TakeCareChemistry, ChemistryOpenEhrData>(tcData);
                    var result = await _chemistryOpenEhrLoader.Load<OpenEhrData<OpenEhrChemistry>, object>(new OpenEhrData<OpenEhrChemistry>()
                    {
                        PatientID = tcOpenEhrData.PatientID,
                        Compositions = tcOpenEhrData.ChemistryData
                    });  // ToDo Mapper and model optimization
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
