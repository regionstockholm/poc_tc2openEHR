using Spine.Foundation.Web.OpenEhr.Client;
using Spine.Migration.OpenEhr.Etl.Core;

namespace Spine.Migration.OpenEhr.Loader
{
    public interface IOpenEhrLoader : ILoader
    {
        OpenEhrConfigurations OpenEhrConfiguratoins { get; }
    }
}
