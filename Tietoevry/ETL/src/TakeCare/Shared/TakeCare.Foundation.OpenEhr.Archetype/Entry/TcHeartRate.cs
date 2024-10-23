using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcHeartRate : BaseMeasurement
    {
        public Keyword Keyword { get; set; }
        public TcHeartRate(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            Measurement = new Measurement();
            var keywordPrefix = $@"{prefix}/hjärtfrekvens";
            this._prefix = keywordPrefix;

            Keyword = new Keyword($@"{keywordPrefix}:{occurance}");
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                     ""{_prefix}:{_occurance}/frekvens|magnitude"": ""{Measurement.Magnitude}"",
                     ""{_prefix}:{_occurance}/frekvens|unit"": ""{Measurement.Units}"",
                    {Keyword.ToString()}";
        }
    }
}
