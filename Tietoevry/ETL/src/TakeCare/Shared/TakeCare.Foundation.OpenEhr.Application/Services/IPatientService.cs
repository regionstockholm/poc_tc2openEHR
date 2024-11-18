using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public interface IPatientService
    {
        PatientDetails GetPatient(string ssn);
    }
}
