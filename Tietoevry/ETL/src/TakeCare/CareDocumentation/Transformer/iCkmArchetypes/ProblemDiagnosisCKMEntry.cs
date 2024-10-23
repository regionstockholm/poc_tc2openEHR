using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

internal record ProblemDiagnosisCKMEntry
{
    private static IUnitProvider _unitService;
    private static ITerminologyProvider _lookupProvider;
    //static constructor
    static ProblemDiagnosisCKMEntry()
    {
        _unitService = new UnitProvider();
        _lookupProvider = new TerminologyProvider();
    }

    public static void AddProblemDiagnosisData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix, TerminologyDetails termData)
    {
        StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
        prefixBuilder.Append("ickm/diagnos_enl_icd-10");
        prefixBuilder.Append(":");
        string prefix = prefixBuilder.ToString();
        composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
        string suffix = "/sökord/";
        TerminologyDetails termCatalog = _lookupProvider.GetTerminology(keyword.TermId);

        composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
        composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = termData.TermId;
        composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = termData.TermName;
        composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = termData.Terminology;
        composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = termData.Datatype;

        if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) {
            composedObject[$"{prefix}{v}{"/problem_diagnos_namn"}"] = keyword.Value.TextVal;
            string[] diagnosData = keyword.Value.TextVal.Split(" ");
            if (diagnosData.Length > 1)
            {
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|code"}"] = diagnosData[0];
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|value"}"] = string.Join(" ", diagnosData.Skip(1));
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|terminology"}"] = "ICD";
            }
            else
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
