using Newtonsoft.Json;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;


namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Service
{
    public class MedicationService : IMedicationService
    {
        private static List<EquivalenceModel> _equivalenceDetails { get; set; }

        public EquivalenceModel GetEquivalenceDetails(string isReplaceble)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "equivalence-details.json");
            string jsonData = File.ReadAllText(filePath);
            _equivalenceDetails = JsonConvert.DeserializeObject<List<EquivalenceModel>>(jsonData);
            return _equivalenceDetails.Find(t => t.IsReplaceble == isReplaceble);
        }
    }
}
