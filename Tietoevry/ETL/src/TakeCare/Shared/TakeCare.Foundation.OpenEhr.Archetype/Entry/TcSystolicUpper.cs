using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcSystolicUpper : SystolicUpper
    {
        public Keyword Keyword { get; set; }

        public TcSystolicUpper(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            this._occurance = occurance;
            Systolic = new Measurement();
            var keywordPrefix = $@"{prefix}/blodtryck_systoliskt_-_övre";
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
