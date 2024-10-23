using System.Text;
using Newtonsoft.Json.Linq;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record BloodPressure
    {
        private static IUnitProvider _unitService;

        //static constructor
        static BloodPressure()
        {
            _unitService = new UnitProvider();
        }

        public static void AddBloodPressureData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix, TerminologyDetails termData)
        {   
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/blodtryck:");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            string suffix = "/sökord/";
            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
            if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal))
            {
                string[] bpValue = keyword.Value.TextVal.Split('/');
                if (bpValue.Length > 0) { 
                    composedObject[$"{prefix}{v}{"/systoliskt|magnitude"}"] = bpValue[0];
                    composedObject[$"{prefix}{v}{"/systoliskt|unit"}"] = "mm[Hg]";
                }
                if (bpValue.Length > 1)
                {
                    composedObject[$"{prefix}{v}{"/diastoliskt|magnitude"}"] = bpValue[1];
                    composedObject[$"{prefix}{v}{"/diastoliskt|unit"}"] = "mm[Hg]";
                }
            }

            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = termData.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = termData.TermName;
            composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = termData.Terminology;
            composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = termData.Datatype;

            if (keyword.Value != null)
            {
                if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                {
                    composedObject[$"{prefix}{v}{suffix}{"värde/text_value"}"] = keyword.Value.TextVal;
                }
            }

            if (!string.IsNullOrEmpty(keyword.Comment))
            {
                composedObject[$"{prefix}{v}{suffix}{"kommentar"}"] = keyword.Comment;
            }
            composedObject[$"{prefix}{v}{suffix}{"nivå"}"] = keyword.ParentCount;
            if (keyword.Children != null)
            {
                for (int i =0; i < keyword.Children.Count; i++){
                    composedObject[$"{prefix}{v}{suffix}{"underordnat_sökord:"}{i}{"/ehr_uri_value"}"] = "ehr://" + keyword.Children[i];
                }
            }    
        }
    }
}
