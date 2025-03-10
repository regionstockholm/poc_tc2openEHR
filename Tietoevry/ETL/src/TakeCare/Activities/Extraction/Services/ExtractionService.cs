using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TakeCare.Migration.OpenEhr.Activities.Extraction.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Services
{
    internal class ExtractionService : IExtractionService
    {
        public TakeCareActivities ExtractActivitiesData(string file)
        {
            var activitiesData = new TakeCareActivities();
            var fileName = Path.GetFileNameWithoutExtension(file);
            string[] fileNameStrings = fileName.Split('_');
            activitiesData.PatientId = fileNameStrings[1];

            var jsonContent = File.ReadAllText(file);
            var token = JToken.Parse(jsonContent);
            if (token is JArray)
            {
                var responses = JsonConvert.DeserializeObject<List<Activity>>(jsonContent);
                if (responses != null)
                {
                    activitiesData.Activities.AddRange(responses);
                }
            }
            else if (token is JObject)
            {
                var response = JsonConvert.DeserializeObject<Activity>(jsonContent);
                if(response!=null)
                {
                    activitiesData.Activities.Add(response);
                }
            }
            return activitiesData;
        }
    }
}
