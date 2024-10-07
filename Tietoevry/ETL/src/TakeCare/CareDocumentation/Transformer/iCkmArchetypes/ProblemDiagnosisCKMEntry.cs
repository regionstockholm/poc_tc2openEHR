using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

internal record ProblemDiagnosisCKMEntry
{
    private readonly IUnitProvider _unitService;

    public ProblemDiagnosisCKMEntry(IUnitProvider unitService)
    {
        _unitService = unitService;
    }

    public static void AddProblemDiagnosisData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix)
    {
        StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
        prefixBuilder.Append("ickm/problem_diagnos");
        prefixBuilder.Append(":");
        string prefix = prefixBuilder.ToString();
        composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
        if (keyword.Value != null) {
            composedObject[$"{prefix}{v}{"/problem_diagnos_namn"}"] = keyword.Name;
        }
        
        string suffix = "/sökord/";
        composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
        composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = keyword.TermId;
        composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = keyword.Name;
        composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = "TC-Datatypes";


        composedObject[$"{prefix}{v}{suffix}{"dv_text_en"}"] = "*DV_TEXT (en) 54";
        composedObject[$"{prefix}{v}{suffix}{"dv_boolean_en"}"] = false;


        if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal))
        {
            //composedObject[$"{prefix}{v}{"/varient_en:0"}"] = keyword.Value.TextVal;
            composedObject[$"{prefix}{v}{suffix}{"värde/text_value"}"] = keyword.Value.TextVal;
            composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = "TextVal";
        }
        if (string.IsNullOrEmpty(keyword.Comment))
        {
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
