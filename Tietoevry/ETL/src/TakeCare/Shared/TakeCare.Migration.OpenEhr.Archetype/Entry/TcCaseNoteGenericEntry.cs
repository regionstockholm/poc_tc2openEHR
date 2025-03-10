using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    public class TcCaseNoteGenericEntry : BaseEntry
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcCaseNoteGenericEntry(string prefix, string occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance.ToString());
            _prefix = prefix;
            _occurance = occurance;
            var keywordPrefix = $@"{prefix}:{occurance}";
            Keyword = new KeywordCaseNote(keywordPrefix);
        }
        public override string ToString()
        {
            return $@"
                            ""{_prefix}:{_occurance}/_uid"": ""{Uid}"",{Keyword.ToString()}";
        }
    }
}
