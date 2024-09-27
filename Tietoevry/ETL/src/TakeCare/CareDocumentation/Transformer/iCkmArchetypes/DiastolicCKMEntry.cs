using System.Text;
using Newtonsoft.Json.Linq;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record DiastolicCKMEntry
    {
        public static void AddDiastolicData(JObject composedObject, KeywordDto keyword, int v, string keywordname, string commonPrefix)
        {   
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/");
            prefixBuilder.Append("blodtryck_1");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            composedObject[$"{prefix}{v}{"/diastoliskt|magnitude"}"] = (keyword.Value != null) ? ((keyword.Value.NumVal != null) ? keyword.Value.NumVal.Val : keyword.Value.TextVal) : "";
            composedObject[$"{prefix}{v}{"/diastoliskt|unit"}"] = "mm[Hg]"; 
            composedObject[$"{prefix}{v}{"/time"}"] = DateTime.UtcNow.ToString("o");
            string suffix = "/sökord/";
            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = keyword.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = keyword.Name;
            composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = "external_terminology";
            /*if ( string.IsNullOrWhiteSpace(keyword.Value))
            {
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|code"}"] = keyword.TermId;
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|value"}"] = "";
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|terminology"}"] = "external_terminology";
            }
            composedObject[$"{prefix}{v}{suffix}{"datatyp|code"}"] = "";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|value"}"] = "";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|terminology"}"] = "external_terminology";
            composedObject[$"{prefix}{v}{suffix}{"egenskaper:0|code"}"] = "";
            */
            if (keyword.Value != null && keyword.Value.NumVal!=null && keyword.Value.NumVal.Unit!=null)
            {
                composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
            }
            composedObject[$"{prefix}{v}{suffix}{"kommentar"}"] = keyword.Comment;
            composedObject[$"{prefix}{v}{suffix}{"nivå"}"] = keyword.ParentCount;
            if (keyword.Childs != null)
            {
                for (int i =0; i < keyword.Childs.Count; i++){
                    composedObject[$"{prefix}{v}{suffix}{"underordnat_sökord:"}{i}"] = "ehr://" + keyword.Childs[i];
                }
            }    
        }
    }
}
