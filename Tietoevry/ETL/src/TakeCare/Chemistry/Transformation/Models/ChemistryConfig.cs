using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class ChemistryConfig
    {
        public EhrConfig Ehr { get; set; }

        public Template Template { get; set; }

        public Language Language { get; set; }
    }
}