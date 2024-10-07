using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record SystolicUpper
    {
        private readonly IUnitProvider _unitService;

        public SystolicUpper(IUnitProvider unitService)
        {
            _unitService = unitService;
        }

        public static void AddSystolicUpperData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix); 
            prefixBuilder.Append("ickm/blodtryck_systoliskt_-_övre");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            string suffix = "/sökord/";
            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
            if (keyword.Value != null && keyword.Value.NumVal!=null)
            {
                composedObject[$"{prefix}{v}{"/systoliskt|magnitude"}"] = keyword.Value.NumVal.Val;
                if (keyword.Value.NumVal.Unit != null)
                {
                    composedObject[$"{prefix}{v}{"/systoliskt|unit"}"] = "mm[Hg]"; //verify
                    composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
                }  
            }

            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = keyword.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = keyword.Name;

            if (keyword.Value != null)
            {
                /*if (!string.IsNullOrEmpty(keyword.Value.TermId))
                {
                    composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|code"}"] = keyword.Value.TermId;
                    if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                    {
                        composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|value"}"] = keyword.Value.TextVal;
                    }
                    composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|terminology"}"] = "TC-Datatypes";
                    composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = "TermId";
                }
                else */
                if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                {
                    composedObject[$"{prefix}{v}{suffix}{"värde/text_value"}"] = keyword.Value.TextVal;
                    composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = "TextVal";
                }
                else if (keyword.Value.NumVal != null && keyword.Value.NumVal.Val != null)
                {
                    composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|magnitude"}"] = keyword.Value.NumVal.Val;
                    if (keyword.Value.NumVal.Unit != null)
                    {
                        composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|unit"}"] = keyword.Value.NumVal.Unit;
                        composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
                    }
                    composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = "NumVal";
                }
            }


            composedObject[$"{prefix}{v}{suffix}{"dv_text_en"}"] = "*DV_TEXT (en) 54";
            composedObject[$"{prefix}{v}{suffix}{"dv_boolean_en"}"] = false;


            if (!string.IsNullOrEmpty(keyword.Comment)){ 
                composedObject[$"{prefix}{v}{suffix}{"kommentar"}"] = keyword.Comment;
            }
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
