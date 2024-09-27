using Newtonsoft.Json;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class TerminologyLookup : ITerminolgyLookup
    {
        string filePath;
        public TerminologyLookup()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Combine it with the relative path to the assets folder
            filePath = Path.Combine(currentDirectory, "Assets", "Terminology.json");
        }
        public Dictionary<string, string> GetTerminology()
        {
            //lookup term
            // Read the JSON file
            string jsonData = File.ReadAllText(filePath);

            // Parse the JSON data into a JToken
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            return dictionary;
        }

    }
}
