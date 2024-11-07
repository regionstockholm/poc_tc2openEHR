namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class Keyword
    {
        private string _prefix;
        public string Code { get; set; }
        public string Value { get; set; }
        public string Terminology { get; set; }
        public string Datatype { get; set; }
        public string OriginalUnit { get; set; }
        public string Comment { get; set; }
        public int Level { get; set; }
        public string EhrUriValue { get; set; }
        public Guid EntryUid { get; set; }
        public string TextValue { get; set; }
        public Keyword(string prefix)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            _prefix = prefix;
        }

        public override string ToString()
        {
            var result = $@"""{_prefix}/sökord/namn|code"": ""{Code}"",
                            ""{_prefix}/sökord/namn|value"": ""{Value}"",
                            ""{_prefix}/sökord/namn|terminology"": ""{Terminology}"",
                            ""{_prefix}/sökord/datatyp"": ""{Datatype}"",
                            ""{_prefix}/sökord/nivå"": {Level},
                            ""{_prefix}/sökord/entry_uid"": ""{EntryUid}"",
                           ";
            return result;
            //return string.IsNullOrWhiteSpace(EhrUriValue) ?
            //    result : $@"{result} ""{_prefix}/sökord/underordnat_sökord:0/ehr_uri_value"": ""{EhrUriValue}"",";
        }
    }

}
