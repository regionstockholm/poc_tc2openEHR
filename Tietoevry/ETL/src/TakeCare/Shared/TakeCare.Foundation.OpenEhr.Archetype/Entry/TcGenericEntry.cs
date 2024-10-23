using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcGenericEntry : BaseEntry
    {
        public Keyword Keyword { get; set; }
        public TcGenericEntry(string prefix, string occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance.ToString());
            _prefix = prefix;
            _occurance = occurance;
            var keywordPrefix = $@"{prefix}/genrisk_händelse:{occurance}";
            Keyword = new Keyword(keywordPrefix);
        }
        public override string ToString()
        {
            var result = $@"
                            ""{_prefix}/genrisk_händelse:{_occurance}/_uid"": ""{Uid}"",";

            return $@"{result}
                      {Keyword.ToString()}";
        }
    }
}
