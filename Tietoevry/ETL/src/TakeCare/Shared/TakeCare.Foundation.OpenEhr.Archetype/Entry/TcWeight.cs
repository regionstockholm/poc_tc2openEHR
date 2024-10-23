using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcWeight : Weight
    {
        public Keyword Keyword { get; set; }

        public TcWeight(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._prefix = prefix;
            this._occurance = occurance;
            
            WeightValue = new Measurement();
            var keywordPrefix = $@"{prefix}/vikt";
            this._prefix = keywordPrefix;
            Keyword = new Keyword($@"{keywordPrefix}:{occurance}");
        }
        
        public override string ToString()
        {
            return $@"{base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
