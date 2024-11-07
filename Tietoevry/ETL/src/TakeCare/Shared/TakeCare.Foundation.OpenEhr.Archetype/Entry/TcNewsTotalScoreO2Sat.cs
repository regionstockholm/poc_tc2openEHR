using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcNewsTotalScoreO2Sat : NewsTotalScore
    {
        private const string _termName = "news2_totalpoäng_syremättnad2";
        public KeywordCaseNote Keyword { get; set; }
        public TcNewsTotalScoreO2Sat(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            string keywordPrefix = $@"{prefix}/{_termName}";

            this._prefix = keywordPrefix;
            this._occurance = occurance;
            
            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
