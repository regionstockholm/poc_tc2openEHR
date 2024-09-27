using System.Text;
using Newtonsoft.Json.Linq;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class TemplateServices : ITemplateServices
    {

        string filePath;
        public TemplateServices()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Combine it with the relative path to the assets folder
            filePath = Path.Combine(currentDirectory, "Assets", "Templatev1.json");
        }

        public Dictionary<string, Object> GetTemplate()
        {

            // Read the JSON file
            string jsonData = File.ReadAllText(filePath);

            // Parse the JSON data into a JToken
            JToken data = JToken.Parse(jsonData.ToLower());

            // Create the object
            var structuredObject = DivideData(data);

            // Print the created object
            //PrintStructuredObject(structuredObject);

            return structuredObject;
        }


        // Function to recursively divide data based on keys and subkeys
        static Dictionary<string, object> DivideData(JToken token)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (JProperty property in token.Children<JProperty>())
            {
                string[] parts = property.Name.Split('/');
                var currentDictionary = result;

                for (int i = 0; i < parts.Length - 1; i++)
                {
                    if (!currentDictionary.ContainsKey(parts[i]))
                    {
                        currentDictionary[parts[i]] = new Dictionary<string, object>();
                    }
                    currentDictionary = (Dictionary<string, object>)currentDictionary[parts[i]];
                }

                currentDictionary[parts[^1]] = property.Value.Type == JTokenType.Object ? DivideData(property.Value) : property.Value.ToObject<object>();
            }

            return result;
        }


        // Function to print the structured object
        static void PrintStructuredObject(Dictionary<string, object> data, string indent = "")
        {

            foreach (var kvp in data)
            {
                if (kvp.Value is Dictionary<string, object> subDict)
                {
                    Console.WriteLine($"{indent}{kvp.Key}:");
                    PrintStructuredObject(subDict, indent + "  ");
                }
                else
                {
                    Console.WriteLine($"{indent}{kvp.Key}: {kvp.Value}");
                }
            }
        }

        public string GetCommonPrefix(Dictionary<string, object> template)
        {
            StringBuilder commonPrefix = new StringBuilder();
            while (template.Count == 1)
            {
                var key = template.Keys.First();
                commonPrefix.Append(key);
                commonPrefix.Append("/");
                template = (Dictionary<string, object>)template[key];
            }
            return commonPrefix.ToString();
        }

        public JObject GetJsonData()
        {
            string jsonData = File.ReadAllText(filePath);
            return JObject.Parse(jsonData.ToLower());
        }
    }
}
