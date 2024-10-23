using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record SaturationWithOxygen
    {
        private static IUnitProvider _unitService;
        private static ITerminologyProvider _lookupProvider;
        //static constructor
        static SaturationWithOxygen()
        {
            _unitService = new UnitProvider();
            _lookupProvider = new TerminologyProvider();
        }

        public static void AddSaturationWithOxygenData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix, TerminologyDetails termData)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/saturation_med_syrgas");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            string suffix = "/sökord/";

            TerminologyDetails termCatalog = _lookupProvider.GetTerminology(keyword.TermId);
            if (keyword.Value!=null && keyword.Value.NumVal != null)
            {
                composedObject[$"{prefix}{v}{"/spo|numerator"}"] = keyword.Value.NumVal.Val;
                composedObject[$"{prefix}{v}{"/spo|denominator"}"] = keyword.Value.NumVal.Val;
                composedObject[$"{prefix}{v}{"/spo|type"}"] = 2;

                if (string.IsNullOrEmpty(keyword.Value.NumVal.Unit))
                {
                    if (!string.IsNullOrEmpty(termCatalog.Unit))
                    {
                        composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|magnitude"}"] = keyword.Value.NumVal.Val;
                        composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|unit"}"] = _unitService.GetOpenEhrUnit(termCatalog.Unit); ;
                    }
                    else
                    {
                        throw new System.Exception("Saturation with oxygen : Unit is missing");
                    }
                }
                else
                {
                    string openEhrUnit = _unitService.GetOpenEhrUnit(keyword.Value.NumVal.Unit);
                    composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = keyword.Value.NumVal.Unit;
                    composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|magnitude"}"] = keyword.Value.NumVal.Val;
                    composedObject[$"{prefix}{v}{suffix}{"värde/quantity_value|unit"}"] = openEhrUnit;
                }
                composedObject[$"{prefix}{v}{"/spo"}"] = Utility.CalculatePercentage(keyword.Value.NumVal.Val);
            }
            composedObject[$"{prefix}{v}{"/inandad_syrgas/på_luft"}"] = false;
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
