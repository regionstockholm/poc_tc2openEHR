using TakeCare.Migration.OpenEhr.Activities.Extraction.Models;
using TakeCare.Migration.OpenEhr.Activities.Transformer.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Services
{
    internal interface ICompositionService
    {
        ActivityOpenEhrData Compose(TakeCareActivities inputData);
    }
}
