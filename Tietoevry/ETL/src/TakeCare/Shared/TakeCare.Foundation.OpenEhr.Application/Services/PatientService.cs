using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public class PatientService : IPatientService
    {
        private static List<PatientDetails> _patients { get; set; }
        static PatientService() 
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "PatientDetails.json");
            _patients = Utility.LoadData<List<PatientDetails>>(filePath);
        }

        public PatientDetails GetPatient(string ssn)
        {
            return _patients.Find(patient => patient.SsnId == ssn);
        }
    }
}
