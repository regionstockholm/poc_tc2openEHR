﻿using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(8883, "TC Terminology")]
    public class TcBMI : BaseMeasurement
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcBMI(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            Measurement = new Measurement();
            var keywordPrefix = $@"{prefix}/bmi";
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
