using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcBareWeight : Weight
    {
        public Keyword Keyword { get; set; }

        public TcBareWeight(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            
            WeightValue = new Measurement();
            var updatedPrefix = $@"{prefix}/nakenvikt";
            this._prefix = updatedPrefix;
            this._occurance = occurance;

            Keyword = new Keyword($@"{updatedPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    {base.ToString()}
                    ""vårdkontakt/ickm/nakenvikt:0/klädsel|code"": ""at0013"",
                    ""vårdkontakt/ickm/nakenvikt:0/klädsel|value"": ""Naken"",
                    ""vårdkontakt/ickm/nakenvikt:0/klädsel|terminology"": ""local"",
                    {Keyword.ToString()}";
        }
    }
}
