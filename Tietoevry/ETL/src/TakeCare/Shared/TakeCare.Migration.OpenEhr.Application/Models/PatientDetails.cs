
namespace TakeCare.Migration.OpenEhr.Application.Models
{
    public class PatientDetails
    {
        public string SsnId { get; set; }
        public string PatientId { get; set; }
        public Guid? EhrId { get; set; }
    }
}
