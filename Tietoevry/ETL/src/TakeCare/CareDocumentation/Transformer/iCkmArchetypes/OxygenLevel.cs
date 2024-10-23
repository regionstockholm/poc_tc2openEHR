using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record OxygenLevel
    {
        private static IUnitProvider _unitService;
        private static ITerminologyProvider _lookupProvider;
        //static constructor
        static OxygenLevel()
        {
            _unitService = new UnitProvider();
            _lookupProvider = new TerminologyProvider();
        }

        public static void AddOxygenLevelData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix, TerminologyDetails termData)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/syrgasnivå");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString(); 
            string procent = "/inandad_syrgas/procent_o";
            string suffix = "/sökord/";

            TerminologyDetails termCatalog = _lookupProvider.GetTerminology(keyword.TermId);

            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            if(keyword.Value!=null && keyword.Value.NumVal != null)
            {
                composedObject[$"{prefix}{v}{procent}{"|numerator"}"] = keyword.Value.NumVal.Val;
                composedObject[$"{prefix}{v}{procent}{"|denominator"}"] = 100;
                composedObject[$"{prefix}{v}{procent}{"|type"}"] = 2;

                if (string.IsNullOrEmpty(keyword.Value.NumVal.Unit))
                {
                    if (!string.IsNullOrEmpty(termCatalog.Unit))
                    {
                        composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|magnitude"}"] = keyword.Value.NumVal.Val;
                        composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|unit"}"] = _unitService.GetOpenEhrUnit(termCatalog.Unit); ;
                    }
                    else
                    {
                        composedObject[$"{prefix}{v}{suffix}{"värde/text_value"}"] = keyword.Value.NumVal.Val;
                    }
                }
                else
                {
                    string openEhrUnit = _unitService.GetOpenEhrUnit(keyword.Value.NumVal.Unit);
                    composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
                    composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|magnitude"}"] = keyword.Value.NumVal.Val;
                    composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|unit"}"] = openEhrUnit;
                }
                //composedObject[$"{prefix}{v}{procent}"] = "0.1";// Utility.CalculatePercentage(keyword.Value.NumVal.Val);
            }

            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid;
            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = termData.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = termData.TermName;
            composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = termData.Terminology;
            composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = termData.Datatype;

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
