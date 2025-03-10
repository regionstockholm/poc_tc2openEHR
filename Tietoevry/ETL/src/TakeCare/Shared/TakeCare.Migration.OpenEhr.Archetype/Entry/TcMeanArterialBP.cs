using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(4243, "TC Terminology")]
    public class TcMeanArterialBP : MeanArterialBP
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcMeanArterialBP(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            var keywordPrefix = $@"{prefix}/map";
            this._occurance = occurance;
            this._prefix = keywordPrefix;

            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
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
