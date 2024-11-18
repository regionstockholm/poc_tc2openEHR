using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public class ResultStatusService : IResultStatusService
    {
        private static List<ResultStatusDetails> _statusCodes { get; set; }
        static ResultStatusService() 
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "ResultStatus.json");
            _statusCodes = Utility.LoadData<List<ResultStatusDetails>>(filePath);
        }

        public ResultStatusDetails? GetResult(bool? flag)
        {
            return _statusCodes!=null ?  _statusCodes.Find(status => status.Code.Equals((flag.GetValueOrDefault() ? "true" : "false"))) : null;
        }
    }
}
