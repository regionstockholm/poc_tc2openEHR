﻿using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcInvasiveDiastolic : MeanArterialBP
    {
        public Keyword Keyword { get; set; }
        public string  Method { get; set; }
        public TcInvasiveDiastolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;
            var keywordPrefix = $@"{prefix}/invasivt_blodtryck_diastoliskt:{occurance}";
            this._prefix = keywordPrefix;
            Keyword = new Keyword(prefix);
        }
        public override string ToString()
        {
            return $@"{base.ToString()}
                    ""{_prefix}:{_occurance}/diastoliskt|magnitude"": ""{Measurement.Magnitude}"",
                    ""{_prefix}:{_occurance}/diastoliskt|unit"": ""{Measurement.Units}"",
                    ""{_prefix}:{_occurance}/metod|code"": ""{Method}"",
                     {Keyword.ToString()}";
        }
    }
}
