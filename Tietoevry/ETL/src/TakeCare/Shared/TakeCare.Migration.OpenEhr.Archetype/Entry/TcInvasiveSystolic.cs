using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(4378, "TC Terminology")]
    public class TcInvasiveSystolic : MeanArterialBP
    {
        public KeywordCaseNote Keyword { get; set; }
        public string Method { get; set; }
        public TcInvasiveSystolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            var keywordPrefix = $@"{prefix}/invasivt_blodtryck_systoliskt";
            this._prefix = keywordPrefix;

            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{ occurance}");
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                    ""{_prefix}:{_occurance}/systoliskt|magnitude"": ""{Measurement.Magnitude}"",
                    ""{_prefix}:{_occurance}/systoliskt|unit"": ""{Measurement.Units}"",
                    ""{_prefix}:{_occurance}/metod|code"": ""{Method}"",
                    {Keyword.ToString()}";
        }
    }
}
