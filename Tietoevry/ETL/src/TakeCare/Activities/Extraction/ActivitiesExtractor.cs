using Spine.Migration.OpenEhr.Etl.Core.Models;
using TakeCare.Migration.OpenEhr.Activities.Extraction.Services;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction
{
    public class ActivitiesExtractor : IActivitiesExtractor
    {
        private readonly IExtractionService _extractionService;
        private readonly IFormatService _formatService;

        public ActivitiesExtractor(IExtractionService extractionService,
                                   IFormatService formatService)
        {
            _extractionService = extractionService;
            _formatService = formatService;
        }
        public Task<ExtractionResult<TResult>> Extract<TConfiguration, TResult>(ExtractionConfiguration<TConfiguration> configuration)
            where TConfiguration : class
            where TResult : class
        {
            var activitiesData = _extractionService.ExtractActivitiesData(configuration.Configuration as string);
            activitiesData = _formatService.Format(activitiesData);
            var result = new ExtractionResult<TResult>(activitiesData as TResult);
            return Task.FromResult(result);
        }
    }
}
