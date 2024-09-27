
using Newtonsoft.Json.Linq;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public interface ITemplateServices
    {
        Dictionary<string, object> GetTemplate();
        string GetCommonPrefix(Dictionary<string, object> template);
        JObject GetJsonData();
    }
}