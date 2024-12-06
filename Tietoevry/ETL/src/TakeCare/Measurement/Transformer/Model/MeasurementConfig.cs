using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Measurement.Transformer.Model
{
    public class MeasurementConfig
    {
        public EhrConfig Ehr { get; set; }
        public Template Template { get; set; }
        public FormConfig Form { get; set; }
        public Language Language { get; set; }

    }
}