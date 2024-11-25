using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Services;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction
{
    public class ChemistryExtractor : IChemistryExtractor
    {
        private readonly IExtractionService _extractionService;
        private readonly IFormatService _formatService;

        public ChemistryExtractor(IExtractionService extractionService,
                                  IFormatService formatService)
        {
            _extractionService = extractionService;
            _formatService = formatService;
        }
        public Task<ExtractionResult<TResult>> Extract<TConfiguration, TResult>(ExtractionConfiguration<TConfiguration> configuration)
            where TConfiguration : class
            where TResult : class
        {
            var chemistryData = _extractionService.ExtractChemistryData(configuration.Configuration as string);
            chemistryData = _formatService.Format(chemistryData);
            var result = new ExtractionResult<TResult>(chemistryData as TResult);
            return Task.FromResult(result);
        }
    }
}
