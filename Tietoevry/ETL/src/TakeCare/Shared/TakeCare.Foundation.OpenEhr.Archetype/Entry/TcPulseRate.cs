using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcPulseRate : BaseMeasurement
    {
        public Keyword Keyword { get; set; }
        public TcPulseRate(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            Measurement = new Measurement();
            var updatedPrefix = $@"{prefix}/pulsfrekvens";
            this._prefix = updatedPrefix;
            Keyword = new Keyword($@"{updatedPrefix}:{occurance}");
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
