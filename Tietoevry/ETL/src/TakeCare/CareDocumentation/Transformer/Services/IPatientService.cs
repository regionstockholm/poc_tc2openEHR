using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public interface IPatientService
    {
        PatientDetails GetPatient(string ssn);
    }
}
