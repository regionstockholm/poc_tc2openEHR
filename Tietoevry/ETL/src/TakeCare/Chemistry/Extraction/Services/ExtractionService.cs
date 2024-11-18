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
                foreach (var response in responses)
                {
                    FormatDates(response);
                }
                chemistryReply.ChemistryData.AddRange(responses);
            }
            else if (token is JObject)
            {
                var response = JsonConvert.DeserializeObject<ChemistryReply>(jsonContent);
                FormatDates(response);
                chemistryReply.ChemistryData.Add(response);
            }

            // Print the TakeCareChemistry object
            string jsonString = JsonConvert.SerializeObject(chemistryReply, Formatting.Indented);
            //Console.WriteLine(jsonString);

            return chemistryReply;
        }

        // Format the dates using the ISODateExtension
        private static void FormatDates(ChemistryReply response)
        {
            response.ReplyTime = response.ReplyTime!=null ? response.ReplyTime.ToString().GetFormattedISODate() : null;
            response.SamplingDateTime = response.SamplingDateTime!=null ? response.SamplingDateTime.ToString().GetFormattedISODate() : null;            

            if (response.Attestation != null)
            {
                response.Attestation.CreatedDateTime = response.Attestation.CreatedDateTime !=null ? response.Attestation.CreatedDateTime.ToString().GetFormattedISODate() : null;
                if (response.Attestation.Document != null && response.Attestation.Document.SavedDateTime!=null)
                {
                    response.Attestation.Document.SavedDateTime = response.Attestation.Document.SavedDateTime.ToString().GetFormattedISODate();
                }
                if(response.Attestation.Attested != null && response.Attestation.Attested.DateTime!=null)
                {
                    response.Attestation.Attested.DateTime = response.Attestation.Attested.DateTime.ToString().GetFormattedISODate();
                }
            }

            if (response.Saved != null && response.Saved.SavedTimestamp!=null)
            {
                response.Saved.SavedTimestamp = response.Saved.SavedTimestamp.ToString().GetFormattedISODate();
            }
        }
    }
}
