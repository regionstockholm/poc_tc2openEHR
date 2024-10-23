using Newtonsoft.Json.Linq;
using System.Text;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models
{
    internal record News2TotalScore
    {
        private static IUnitProvider _unitService;
        //static constructor
        static News2TotalScore()
        {
            _unitService = new UnitProvider();
        }

        public static void AddTotalScoreData(JObject composedObject, KeywordDto keyword, int v, string commonPrefix, TerminologyDetails termData)
        {
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("ickm/news2_totalpoäng");
            prefixBuilder.Append(":");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            string suffix = "/sökord/";
            if (keyword.Value != null)
            {
                if (keyword.Value.NumVal != null)
                {
                    if (!string.IsNullOrEmpty(keyword.Value.NumVal.Val))
                    {
                        composedObject[$"{prefix}{v}{"/totalpoäng_news2"}"] = keyword.Value.NumVal.Val;
                        composedObject[$"{prefix}{v}{suffix}{"värde/count_value"}"] = keyword.Value.NumVal.Val;
                    }
                }
                else if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                {
                    composedObject[$"{prefix}{v}{"/totalpoäng_news2"}"] = keyword.Value.TextVal;
                    composedObject[$"{prefix}{v}{suffix}{"värde/text_value"}"] = keyword.Value.TextVal;
                }
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
