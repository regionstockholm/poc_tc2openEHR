using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public interface IPatientService
    {
        PatientDetails GetPatient(string ssn);
    }
}
