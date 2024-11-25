using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Services
{
    internal class ExtractionService : IExtractionService
    {
        public TakeCareChemistry ExtractChemistryData(string file)
        {
            var chemistryReply = new TakeCareChemistry();
            var jsonContent = File.ReadAllText(file);
            var token = JToken.Parse(jsonContent);
            if (token is JArray)
            {
                var responses = JsonConvert.DeserializeObject<List<ChemistryReply>>(jsonContent);                 
                chemistryReply.ChemistryData.AddRange(responses);
            }
            else if (token is JObject)
            {
                var response = JsonConvert.DeserializeObject<ChemistryReply>(jsonContent);
                chemistryReply.ChemistryData.Add(response);
            }

            return chemistryReply;
        }

    }
}
