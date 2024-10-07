using System.Text;
using Newtonsoft.Json.Linq;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record SystolicDiastolicCKMEntry
    {
        private readonly IUnitProvider _unitService;

        public SystolicDiastolicCKMEntry(IUnitProvider unitService)
        {
            _unitService = unitService;
        }

        public static void AddSystolicDiastolicData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix)
        {
            if(keyword.Value!=null && !string.IsNullOrEmpty(keyword.Value.TextVal))
            {
                string[] bpValue = keyword.Value.TextVal.Split('/');
                keyword.Value.NumVal = new Extraction.Model.CareDoc.NumVal { Val = bpValue[0] };
                SystolicUpper.AddSystolicUpperData(composedObject, keyword, v, commonPrefix);
                if (bpValue.Length > 1)
                {
                    keyword.Value.NumVal = new Extraction.Model.CareDoc.NumVal { Val = bpValue[1] };
                    DiastolicLower.AddDiastolicLowerData(composedObject, keyword, v, commonPrefix);
                }
            }
        }
    }
}
