using Newtonsoft.Json.Linq;

namespace Spine.Foundation.Web.OpenEhr.Client
{
    public interface IOpenEhrServiceAgent
    {
        Task<Guid> GetEhrId(string extenalId, string nameSpace);
        Task<dynamic> SaveComposition(JObject composition, Guid ehrId);
    }
}