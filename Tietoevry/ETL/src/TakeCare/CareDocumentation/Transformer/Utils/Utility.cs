using Newtonsoft.Json;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils
{
    internal static class Utility
    {
        public static T LoadData<T>(string filePath)
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
        public static string CalculatePercentage(string val)
        {
            //parse the val string into number
            decimal number = Convert.ToDecimal(val);
            return (number / 100).ToString();
        }
    }
}
