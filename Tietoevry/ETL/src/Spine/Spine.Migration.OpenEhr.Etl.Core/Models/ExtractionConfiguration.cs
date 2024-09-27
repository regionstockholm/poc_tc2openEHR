namespace Spine.Migration.OpenEhr.Etl.Core.Models
{
    public class ExtractionConfiguration<TConfiguration>
    {
        public ExtractionConfiguration(TConfiguration configuration)
        {
            Configuration = configuration;
        }
        public TConfiguration Configuration { get; }
    }
}
