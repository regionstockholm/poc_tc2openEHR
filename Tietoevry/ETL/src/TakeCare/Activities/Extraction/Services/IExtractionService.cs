using TakeCare.Migration.OpenEhr.Activities.Extraction.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Services
{
    public interface IExtractionService
    {
        TakeCareActivities ExtractActivitiesData(string file);
    }
}
