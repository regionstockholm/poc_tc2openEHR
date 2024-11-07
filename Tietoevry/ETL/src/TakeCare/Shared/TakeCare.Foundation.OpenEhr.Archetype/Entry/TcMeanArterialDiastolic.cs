using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcMeanArterialDiastolic : MeanArterialBP
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcMeanArterialDiastolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            var keywordPrefix = $@"{prefix}/invasivt_blodtryck_diastoliskt";
            this._prefix = keywordPrefix;
            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                    ""{_prefix}:{_occurance}/diastoliskt|magnitude"": ""{Measurement.Magnitude}"",
                    ""{_prefix}:{_occurance}/diastoliskt|unit"": ""{Measurement.Units}"",
                    ""{_prefix}:{_occurance}/metod|code"": ""{Language.Code}"",
                     {Keyword.ToString()}";
        }
    }
}
