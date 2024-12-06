using TakeCare.Migration.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public interface ITerminologyProvider
    {
        TerminologyDetails GetTerminology(string termId);
    }
}
