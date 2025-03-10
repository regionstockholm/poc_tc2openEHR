using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(1965, "TC Terminology")]
    public class TcWeight : Weight
    {
        public KeywordCaseNote Keyword { get; set; }

        public TcWeight(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._prefix = prefix;
            this._occurance = occurance;
            
            WeightValue = new Measurement();
            var keywordPrefix = $@"{prefix}/vikt";
            this._prefix = keywordPrefix;
            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }
        
        public override string ToString()
        {
            return $@"{base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
