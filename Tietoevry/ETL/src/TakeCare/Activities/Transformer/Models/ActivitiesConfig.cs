using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class ActivitiesConfig
    {
        public EhrConfig Ehr { get; set; }

        public Template Template { get; set; }

        public FormConfig Form { get; set; }

        public Language Language { get; set; }
    }
}
