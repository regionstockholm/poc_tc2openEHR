using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.iCkmArchetypes
{
    internal record A24HourlyBPCurve
    {
        private static readonly IUnitProvider _unitService;

        static A24HourlyBPCurve()
        {
            _unitService = new UnitProvider();
        }
        public static void AddA24HourlyBPCurveData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix, TerminologyDetails termData)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/a24-timmars_blodtryckskurva");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            string midword = "/a24_timmars_blodtrycksmätning";
            string suffix = "/sökord/";
            composedObject[$"{prefix}{v}{midword}{"/_uid"}"] = keyword.Guid;
            composedObject[$"{prefix}{v}{midword}{"/math_function|code"}"] = "146"; //verify
            composedObject[$"{prefix}{v}{midword}{"/math_function|value"}"] = "mean"; //verify
            composedObject[$"{prefix}{v}{midword}{"/math_function|terminology"}"] = "openehr"; //verify
            composedObject[$"{prefix}{v}{midword}{"/width"}"] = "P2DT4H18M"; //verify
            if(keyword.Value!=null && !string.IsNullOrEmpty(keyword.Value.TextVal))
            {
                string[] bpValue = keyword.Value.TextVal.Split('/');
                if (bpValue.Length > 0)
                {
                    composedObject[$"{prefix}{v}{midword}{"/systoliskt|magnitude"}"] = bpValue[0];
                    //composedObject[$"{prefix}{v}{midword}{"/systoliskt|unit"}"] = "mm[Hg]";
                }
                if (bpValue.Length > 1)
                {
                    composedObject[$"{prefix}{v}{midword}{"/diastoliskt|magnitude"}"] = bpValue[1];
                    //composedObject[$"{prefix}{v}{midword}{"/diastoliskt|unit"}"] = "mm[Hg]";
                }
            }

            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
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
                for (int i = 0; i < keyword.Children.Count; i++)
                {
                    composedObject[$"{prefix}{v}{suffix}{"underordnat_sökord:"}{i}{"/ehr_uri_value"}"] = "ehr://" + keyword.Children[i];
                }
            }
        }
    }
}
