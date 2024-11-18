using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public interface ITerminologyProvider
    {
        TerminologyDetails GetTerminology(string termId);
    }
}
