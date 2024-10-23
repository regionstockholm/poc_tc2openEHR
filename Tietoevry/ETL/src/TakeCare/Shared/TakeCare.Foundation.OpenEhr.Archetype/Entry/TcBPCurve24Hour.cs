using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcBPCurve24Hour : BPCurve24Hour
    {
        public Keyword Keyword { get; set; }
        public TcBPCurve24Hour(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            Systolic = new Measurement();
            Diastolic = new Measurement();
            var keywordPrefix = $@"{prefix}/a24-timmars_blodtryckskurva";
            this._prefix = keywordPrefix;
            this._occurance = occurance;
            
            Keyword = new Keyword($@"{keywordPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"{base.ToString()}
                    {Keyword.ToString()}";
         }
    }
}
