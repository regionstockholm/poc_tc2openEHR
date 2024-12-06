using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(2025, "TC Terminology")]
    public class TcBodyTemparature : BaseMeasurement
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcBodyTemparature(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
           
            Measurement = new Measurement();
            var keywordPrefix = $@"{prefix}/kroppstemperatur";
            this._prefix = keywordPrefix;
            this._occurance = occurance;
            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                            ""{_prefix}:{_occurance}/temperatur|magnitude"": ""{Measurement.Magnitude}"",
                            ""{_prefix}:{_occurance}/temperatur|unit"": ""{Measurement.Units}"",
                    {Keyword.ToString()}";
        }
    }
}
