using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(3719, "TC Terminology")]
    public class TcSystolicUpper : SystolicUpper
    {
        public KeywordCaseNote Keyword { get; set; }

        public TcSystolicUpper(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            this._occurance = occurance;
            Systolic = new Measurement();
            var keywordPrefix = $@"{prefix}/blodtryck_systoliskt_-_övre";
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
