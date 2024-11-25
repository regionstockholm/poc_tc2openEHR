using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Services
{
    public interface IFormatService
    {
        TakeCareChemistry Format(TakeCareChemistry response);
    }
}
