using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(8980, "TC Terminology")]
    public class TcBPCurve24Hour : BPCurve24Hour
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcBPCurve24Hour(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            Systolic = new Measurement();
            Diastolic = new Measurement();
            var keywordPrefix = $@"{prefix}/a24-timmars_blodtryckskurva";
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
