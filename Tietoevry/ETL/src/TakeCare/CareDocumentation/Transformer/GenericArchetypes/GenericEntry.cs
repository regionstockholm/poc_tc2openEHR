using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record GenericEntry
    {
        private static IUnitProvider _unitService;
        private static ITerminologyProvider _lookupProvider;

        static GenericEntry()
        {
            _unitService = new UnitProvider();
            _lookupProvider = new TerminologyProvider();
        }
        public static void AddGenericData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix)
        {

            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("genrisk_händelse:");
            string prefix = prefixBuilder.ToString();

            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            string suffix = "/sökord/";

            composedObject[$"{prefix}{v}{suffix}{"entry_uid"}"] = keyword.Guid; // verify
            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = keyword.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = keyword.Name;
            composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = "TC-Datatypes";
            TerminologyDetails termCatalog = _lookupProvider.GetTerminology(keyword.TermId);
            if (keyword.Value != null)
            {
                if (!string.IsNullOrEmpty(keyword.Value.TermId))
                {
                    composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|code"}"] = keyword.Value.TermId;
                    if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                    {
                        composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|value"}"] = keyword.Value.TextVal;
                    }
                    composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|terminology"}"] = "TC-Datatypes";
                }
                else if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                {
                    composedObject[$"{prefix}{v}{suffix}{"värde/text_value"}"] = keyword.Value.TextVal;
                }
                else if (keyword.Value.NumVal != null && keyword.Value.NumVal.Val != null)
                {
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
                }
            }
            TerminologyDetails datatype = _lookupProvider.GetTerminology(keyword.TermId);
            if (datatype != null)
                composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = datatype.Datatype;
            else
                composedObject[$"{prefix}{v}{suffix}{"datatyp"}"] = "Text";

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
