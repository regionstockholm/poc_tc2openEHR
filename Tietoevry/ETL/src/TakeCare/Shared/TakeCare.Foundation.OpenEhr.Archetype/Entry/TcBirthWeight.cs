using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcBirthWeight : Weight
    {
        public Keyword Keyword { get; set; }

        public TcBirthWeight(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            
            WeightValue = new Measurement();
            var updatedPrefix = $@"{prefix}/födelsevikt";
            this._prefix = updatedPrefix;
            this._occurance = occurance;

            Keyword = new Keyword($@"{updatedPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    {base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
