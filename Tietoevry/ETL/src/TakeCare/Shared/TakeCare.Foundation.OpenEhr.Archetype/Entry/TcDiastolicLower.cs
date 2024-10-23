using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcDiastolicLower : DiastolicLower
    {
        public Keyword Keyword { get; set; }

        public TcDiastolicLower(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            this._occurance = occurance;
            Diastolic = new Measurement();
            var keywordPrefix = $@"{prefix}/blodtryck_diastoliskt_-_nedre";
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
