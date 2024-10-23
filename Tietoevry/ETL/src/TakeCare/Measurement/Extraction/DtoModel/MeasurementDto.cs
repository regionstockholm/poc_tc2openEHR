using TakeCare.Migration.OpenEhr.Measurement.Extraction.Extension;
using TakeCare.Migration.OpenEhr.Measurement.Extraction.Model;

namespace TakeCare.Migration.OpenEhr.Measurement.Extraction.DtoModel
{
    public class MeasurementDto
    {
        public string PatientId { get; set; }
        public string CreatedByUserId { get; set; }

        private string _createdOn;
        public string CreatedOn {
            get
            {
                return _createdOn;
            }
            set
            {   
                _createdOn = value.GetFormattedISODate();
            }
        }

        public UserDetails CreatedBy { get; set; }
        public UserDetails SavedBy { get; set; }
        public List<TakeCare.Migration.OpenEhr.Measurement.Extraction.Model.Measurement> Measurements { get; set; }
    }
}
