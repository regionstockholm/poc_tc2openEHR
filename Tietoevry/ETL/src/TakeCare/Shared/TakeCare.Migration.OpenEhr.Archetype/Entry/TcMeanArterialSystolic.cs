﻿using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    public class TcMeanArterialSystolic : MeanArterialBP
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcMeanArterialSystolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            var keywordPrefix = $@"{prefix}/invasivt_blodtryck_systoliskt";
            this._prefix = keywordPrefix;

            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                    ""{_prefix}:{_occurance}/systoliskt|magnitude"": ""{Measurement.Magnitude}"",
                    ""{_prefix}:{_occurance}/systoliskt|unit"": ""{Measurement.Units}"",
                    ""{_prefix}:{_occurance}/metod|code"": ""{Language.Code}"",
                    {Keyword.ToString()}";
        }
    }
}
