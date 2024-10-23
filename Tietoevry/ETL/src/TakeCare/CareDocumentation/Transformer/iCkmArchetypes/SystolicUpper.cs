using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record SystolicUpper
    {

        private static IUnitProvider _unitService;
        private static ITerminologyProvider _lookupProvider;
        //static constructor
        static SystolicUpper()
        {
            _unitService = new UnitProvider();
            _lookupProvider = new TerminologyProvider();
        }

        public static void AddSystolicUpperData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix, TerminologyDetails termData)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix); 
            prefixBuilder.Append("ickm/blodtryck_systoliskt_-_övre");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            string suffix = "/sökord/";
            TerminologyDetails termCatalog = _lookupProvider.GetTerminology(keyword.TermId);
            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
            if (keyword.Value != null && keyword.Value.NumVal!=null)
            {
                if (keyword.Value.NumVal != null)
                {
                    composedObject[$"{prefix}{v}{"/systoliskt|magnitude"}"] = keyword.Value.NumVal.Val;
                    composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|magnitude"}"] = keyword.Value.NumVal.Val;
                    if (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit))
                    {
                        string openEhrUnit = _unitService.GetOpenEhrUnit(keyword.Value.NumVal.Unit);
                        composedObject[$"{prefix}{v}{"/systoliskt|unit"}"] = openEhrUnit;
                        composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|unit"}"] = openEhrUnit;
                        composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
                    }
                    else if (!string.IsNullOrEmpty(termCatalog.Unit))
                    {
                        string openEhrUnit = _unitService.GetOpenEhrUnit(termCatalog.Unit);
                        composedObject[$"{prefix}{v}{"/systoliskt|unit"}"] = openEhrUnit;
                        composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|unit"}"] = openEhrUnit;
                    }
                    else
                    {
                        throw new System.Exception("SystolicUpper : Unit is missing");
                    }
                }
            }

            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = termData.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = termData.TermName;
            composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = termData.Terminology;
            composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = termData.Datatype;

            if (!string.IsNullOrEmpty(keyword.Comment)){ 
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
