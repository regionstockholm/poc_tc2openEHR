using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class MedicationConfig
    {
        public EhrConfig Ehr { get; set; }

        public Template Template { get; set; }

        public FormConfig Form { get; set; }

    }
}
