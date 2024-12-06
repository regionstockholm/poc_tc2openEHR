using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(11274, "TC Terminology")]
    public class TcNewsTotalScoreHeartRate : NewsTotalScore
    {
        private const string _termName = "news2_totalpoäng_hjärtfrekvens";
        public KeywordCaseNote Keyword { get; set; }
        public TcNewsTotalScoreHeartRate(string prefix, string occurance) : base(prefix, occurance)
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
