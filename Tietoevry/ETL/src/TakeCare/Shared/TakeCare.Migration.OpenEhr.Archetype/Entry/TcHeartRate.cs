using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(11140, "TC Terminology")]
    public class TcHeartRate : BaseMeasurement
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcHeartRate(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            Measurement = new Measurement();
            var keywordPrefix = $@"{prefix}/hjärtfrekvens";
            this._prefix = keywordPrefix;

            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
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
