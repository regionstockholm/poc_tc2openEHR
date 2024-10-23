using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcOxygenSaturation : O2Saturation
    {
        public Keyword Keyword { get; set; }
        public TcOxygenSaturation(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            Measurement = new MeasurementFraction();
            var keywordPrefix = $@"{prefix}/saturation_med_syrgas";
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
