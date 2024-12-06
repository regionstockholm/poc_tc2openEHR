using TakeCare.Migration.OpenEhr.Application.Utils;
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
        public string CareUnitId { get; set; }
        public List<TakeCare.Migration.OpenEhr.Measurement.Extraction.Model.Measurement> Measurements { get; set; }
        public string LinkCode { get; set; }
        public string VersionId { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string EventDateTime { get; set; }
        public string DocId { get; set; }
    }
}
