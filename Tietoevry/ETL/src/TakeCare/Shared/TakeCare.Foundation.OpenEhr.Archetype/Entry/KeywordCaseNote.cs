namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class KeywordCaseNote : Keyword
    {
        private string _prefix;
        public List<Guid> EhrUriValues { get; set; }
        //public Guid EntryUid { get; set; }
        public string TermIDValue { get; set; }
        public decimal? NumValue { get; set; }
        public string? NumUnit { get; set; }


        public KeywordCaseNote(string prefix) : base(prefix) 
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            _prefix = prefix;
        }

        public override string ToString()
        {
            var result = $@"
                            {base.ToString()}";

            if (NumValue != null)
            {
                if (NumUnit != null)
                {
                    result += $@"
                            ""{_prefix}/sökord/värde/quantity_value|magnitude"": {NumValue},
                            ""{_prefix}/sökord/värde/quantity_value|unit"": ""{NumUnit}"",";
                }
                else
                {
                    result += $@"
                            ""{_prefix}/sökord/värde/text_value"": ""{NumValue}"",";
                }
            }
            else if (!string.IsNullOrWhiteSpace(TermIDValue))
            {
                result += $@"
                            ""{_prefix}/sökord/värde/coded_text_value|code"": ""{TermIDValue}"",
                            ""{_prefix}/sökord/värde/coded_text_value|terminology"": ""TC-Datatypes"",";
                if(!string.IsNullOrEmpty(TextValue))
                    result += $@"
                            ""{_prefix}/sökord/värde/coded_text_value|value"": ""{TextValue}"",";
            }
            else if (!string.IsNullOrWhiteSpace(TextValue))
            {
                result += $@"
                            ""{_prefix}/sökord/värde/text_value"": ""{TextValue}"",";
            }

            if (!string.IsNullOrWhiteSpace(OriginalUnit))
            {
                result += $@"
                            ""{_prefix}/sökord/originalenhet"": ""{OriginalUnit}"",";
            }
            if (!string.IsNullOrWhiteSpace(Comment))
            {
                result += $@"
                            ""{_prefix}/sökord/kommentar"": ""{Comment}"",";
            }
            if (EhrUriValues != null && EhrUriValues.Count > 0)
            {
                for (int i = 0; i < EhrUriValues.Count; i++)
                {
                    result += $@"
                            ""{_prefix}/sökord/underordnat_sökord:{i}/ehr_uri_value"": ""ehr://{EhrUriValues[i].ToString()}"",";
                }
            }
            else
            {
                result += $@"
                            ""{_prefix}/sökord/underordnat_sökord:0/ehr_uri_value"": """",";
            }
            return result;
        }
    }

}
