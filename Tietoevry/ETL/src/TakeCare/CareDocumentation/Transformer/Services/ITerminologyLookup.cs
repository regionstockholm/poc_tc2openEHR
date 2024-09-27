
namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public interface ITerminolgyLookup
    {
        Dictionary<string, string> GetTerminology();
    }
}