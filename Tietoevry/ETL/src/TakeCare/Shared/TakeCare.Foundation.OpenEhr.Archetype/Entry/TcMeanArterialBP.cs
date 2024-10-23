using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcMeanArterialBP : MeanArterialBP
    {
        public Keyword Keyword { get; set; }
        public TcMeanArterialBP(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            var keywordPrefix = $@"{prefix}/map";
            this._occurance = occurance;
            this._prefix = keywordPrefix;

            Keyword = new Keyword($@"{keywordPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    {base.ToString()}
                            ""{_prefix}:{_occurance}/medelartärtryck|magnitude"": ""{Measurement.Magnitude}"",
                            ""{_prefix}:{_occurance}/medelartärtryck|unit"": ""{Measurement.Units}"",
                     {Keyword.ToString()}";
        }
    }
}
