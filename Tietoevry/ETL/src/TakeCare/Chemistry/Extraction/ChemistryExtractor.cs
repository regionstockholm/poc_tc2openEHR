using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Services;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction
{
    public class ChemistryExtractor : IChemistryExtractor
    {
        private readonly IExtractionService _extractionService;

        public ChemistryExtractor(IExtractionService extractionService)
        {
            _extractionService = extractionService;
        }
        public Task<ExtractionResult<TResult>> Extract<TConfiguration, TResult>(ExtractionConfiguration<TConfiguration> configuration)
            where TConfiguration : class
            where TResult : class
        {
            var chemistryData = _extractionService.ExtractChemistryData(configuration.Configuration as string);
            var result = new ExtractionResult<TResult>(chemistryData as TResult);
            return Task.FromResult(result);
        }
    }
}
