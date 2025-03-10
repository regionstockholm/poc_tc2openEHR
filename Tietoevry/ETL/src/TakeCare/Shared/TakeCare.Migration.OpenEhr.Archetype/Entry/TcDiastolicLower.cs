using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(3720, "TC Terminology")]
    public class TcDiastolicLower : DiastolicLower
    {
        public KeywordCaseNote Keyword { get; set; }

        public TcDiastolicLower(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            this._occurance = occurance;
            Diastolic = new Measurement();
            var keywordPrefix = $@"{prefix}/blodtryck_diastoliskt_-_nedre";
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
