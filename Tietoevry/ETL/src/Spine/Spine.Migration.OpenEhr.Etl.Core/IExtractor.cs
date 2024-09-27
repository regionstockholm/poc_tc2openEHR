using Spine.Migration.OpenEhr.Etl.Core.Models;

namespace Spine.Migration.OpenEhr.Etl.Core
{

    public interface IExtractor
    {
        Task<ExtractionResult<TResult>> Extract<TConfiguration, TResult>(ExtractionConfiguration<TConfiguration> configuration)
            where TConfiguration : class
            where TResult : class;
    }
}
