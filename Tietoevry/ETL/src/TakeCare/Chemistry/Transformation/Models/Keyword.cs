using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class Keyword
    {
        private readonly string _prefix;
        public string NameCode { get; set; }
        public string NameValue { get; set; }
        public string NameTerminology { get; set; }
        public string ValueQuantityMagnitude { get; set; }
        public string ValueQuantityUnit { get; set; }
        public string? OriginalUnit { get; set; }
        public string? ScaleText { get; set; }
        public string? IsAccredited { get; set; }
        public string? Comment { get; set; }
        public string? EntryUid { get; set; }

        public Keyword(string prefix)
        {
            _prefix = prefix;
        }
        public override string ToString()
        {
            var result = $@"";

            if (!string.IsNullOrWhiteSpace(NameCode))
            {
                result += $@"
                ""{_prefix}/sökord/namn|code"": ""{NameCode}"",
                ""{_prefix}/sökord/namn|value"": ""{NameValue}"",
                ""{_prefix}/sökord/namn|terminology"": ""{NameTerminology}"",";
            }

            if (!string.IsNullOrWhiteSpace(ValueQuantityMagnitude))
            {
                result += $@"
                ""{_prefix}/sökord/värde|quantity_value|magnitude"": ""{ValueQuantityMagnitude}"",
                ""{_prefix}/sökord/värde|quantity_value|unit"": ""{ValueQuantityUnit}"",";
            }

            if (!string.IsNullOrWhiteSpace(OriginalUnit))
            {
                result += $@"
                ""{_prefix}/sökord/originalenhet"": ""{OriginalUnit}"",";
            }

            if (!string.IsNullOrWhiteSpace(ScaleText))
            {
                result += $@"
                ""{_prefix}/sökord/skala_text"": ""{ScaleText}"",";
            }

            if (!string.IsNullOrWhiteSpace(IsAccredited))
            {
                result += $@"
                ""{_prefix}/sökord/ackrediterad"": ""{IsAccredited}"",";
            }

            if (!string.IsNullOrWhiteSpace(Comment))
            {
                result += $@"
                ""{_prefix}/sökord/kommentar"": ""{Comment}"",";
            }

            if (!string.IsNullOrWhiteSpace(EntryUid))
            {
                result += $@"
                ""{_prefix}/sökord/entry_uid"": ""{EntryUid}"",";
            }

            return result;
        }
    }
}
