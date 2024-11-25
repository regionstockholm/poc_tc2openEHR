using Microsoft.Extensions.Logging;
using Spine.Migration.OpenEhr.Etl.Core;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using Spine.Migration.OpenEhr.Loader;
using TakeCare.Migration.OpenEhr.Activities.Extraction;
using TakeCare.Migration.OpenEhr.Activities.Extraction.Models;
using TakeCare.Migration.OpenEhr.Activities.Transformer;
using TakeCare.Migration.OpenEhr.Activities.Transformer.Models;

namespace TakeCare.Migration.OpenEhr.Etl.Handlers
{
    public class ActivityEtlHandler : IEtlHandler
    {
        private readonly Lazy<IActivitiesExtractor> _lazyExtractor;
        private readonly Lazy<IActivitiesTransformer> _lazyTransformer;
        private readonly Lazy<IOpenEhrLoader> _lazyLoader;
        private readonly Lazy<ILogger<ActivityEtlHandler>> _lazyLogger;

        private IActivitiesExtractor _activityExtractor => _lazyExtractor.Value;
        private IActivitiesTransformer _activityTransformer => _lazyTransformer.Value;
        private IOpenEhrLoader _activityOpenEhrLoader => _lazyLoader.Value;
        private ILogger<ActivityEtlHandler> _logger => _lazyLogger.Value;

        public ActivityEtlHandler(Lazy<IActivitiesExtractor> lazyExtractor,
                                  Lazy<IActivitiesTransformer> lazyTransformer, 
                                  Lazy<IOpenEhrLoader> lazyLoader,
                                  Lazy<ILogger<ActivityEtlHandler>> lazyLogger)
        {
            _lazyExtractor = lazyExtractor;
            _lazyTransformer = lazyTransformer;
            _lazyLoader = lazyLoader;
            _lazyLogger = lazyLogger;
        }


        public async Task Execute()
        {
            int count = 0;
            string activityFolder = Path.Combine(AppContext.BaseDirectory, @"Assets\TestData\Activities");
            foreach (var file in Directory.EnumerateFiles(activityFolder, "*.json"))
            {
                try
                {
                    var extractorConfigurations = new ExtractionConfiguration<string>(file);
                    var tcData = await _activityExtractor.Extract<string, TakeCareActivities>(extractorConfigurations);
                    var tcOpenEhrData = await _activityTransformer.Trasform<TakeCareActivities, ActivityOpenEhrData>(tcData);
                    var result = await _activityOpenEhrLoader.Load<OpenEhrData<OpenEhrActivity>, object>(new OpenEhrData<OpenEhrActivity>()
                    {
                        PatientID = tcOpenEhrData.PatientId,
                        Compositions = tcOpenEhrData.ActivityData
                    });  // ToDo Mapper and model optimization                   
                }
                catch (Exception ex)
                {
                    // ToDo Log exceptions in Logger
                    _logger.LogError(ex, $"Error in processing file {file}");

                }
                _logger.LogInformation($"{++count}. File name : {file}");

            }
        }

    }
}
