using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(2275, "TC Terminology")]
    public class TcBMICalculated : BaseMeasurement
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcBMICalculated(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            Measurement = new Measurement();
            var keywordPrefix = $@"{prefix}/bmi_beräknat";
            this._prefix = keywordPrefix;
            this._occurance = occurance;
            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                     ""{_prefix}:{_occurance}/kroppsmasseindex|magnitude"": ""{Measurement.Magnitude}"",
                     ""{_prefix}:{_occurance}/kroppsmasseindex|unit"": ""{Measurement.Units}"",
                    {Keyword.ToString()}";
        }
    }
}
