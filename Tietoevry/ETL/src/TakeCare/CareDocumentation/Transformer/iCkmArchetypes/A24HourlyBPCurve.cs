using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.iCkmArchetypes
{
    internal record A24HourlyBPCurve
    {
        private readonly IUnitProvider _unitService;

        public A24HourlyBPCurve(IUnitProvider unitService)
        {
            _unitService = unitService;
        }
        public static void AddA24HourlyBPCurveData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/a24-timmars_blodtryckskurva");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            string suffix = "/a24_timmars_blodtrycksmätning/";
            composedObject[$"{prefix}{v}{suffix}{"_uid"}"] = keyword.Guid;
            composedObject[$"{prefix}{v}{suffix}{"math_function|code"}"] = "146"; //verify
            composedObject[$"{prefix}{v}{suffix}{"math_function|value"}"] = "mean"; //verify
            composedObject[$"{prefix}{v}{suffix}{"width"}"] = "P2DT4H18M"; //verify
            composedObject[$"{prefix}{v}{suffix}{"systoliskt|magnitude"}"] = (keyword.Value != null) ? ((keyword.Value.NumVal != null) ? keyword.Value.NumVal.Val : keyword.Value.TextVal) : "";
            composedObject[$"{prefix}{v}{suffix}{"systoliskt|unit"}"] = "mm[Hg]";
            composedObject[$"{prefix}{v}{suffix}{"diastoliskt|magnitude"}"] = (keyword.Value != null) ? ((keyword.Value.NumVal != null) ? keyword.Value.NumVal.Val : keyword.Value.TextVal) : "";
            composedObject[$"{prefix}{v}{suffix}{"diastoliskt|unit"}"] = "mm[Hg]";

            suffix = "/sökord/";
            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = keyword.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = keyword.Name;

            /*if ( string.IsNullOrWhiteSpace(keyword.Value))
            {
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|code"}"] = keyword.TermId;
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|value"}"] = "";
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|terminology"}"] = "external_terminology";
            }
            composedObject[$"{prefix}{v}{suffix}{"datatyp|code"}"] = "";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|value"}"] = "";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|terminology"}"] = "external_terminology";
            
            */

            composedObject[$"{prefix}{v}{suffix}{"dv_text_en"}"] = "*DV_TEXT (en) 54";
            composedObject[$"{prefix}{v}{suffix}{"dv_boolean_en"}"] = false;

            if (keyword.Value != null && keyword.Value.NumVal != null && keyword.Value.NumVal.Unit != null)
            {
                composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
            }
            composedObject[$"{prefix}{v}{suffix}{"kommentar"}"] = keyword.Comment;
            composedObject[$"{prefix}{v}{suffix}{"nivå"}"] = keyword.ParentCount;
            if (keyword.Childs != null)
            {
                for (int i = 0; i < keyword.Childs.Count; i++)
                {
                    composedObject[$"{prefix}{v}{suffix}{"underordnat_sökord:"}{i}{"/ehr_uri_value"}"] = "ehr://" + keyword.Childs[i];
                }
            }
        }
    }
}
