using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(11276, "TC Terminology")]
    public class TcNewsScoreHRO2Sat : NewsTotalScore
    {
        private const string _termName = "news2_totalpoäng_syremättnad2_hjärtfrekvens";
        public KeywordCaseNote Keyword { get; set; }
        public TcNewsScoreHRO2Sat(string prefix, string occurance) : base(prefix, occurance)
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
