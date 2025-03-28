﻿using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(2896, "TC Terminology")]
    public class TcBirthWeight : Weight
    {
        public KeywordCaseNote Keyword { get; set; }

        public TcBirthWeight(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            
            WeightValue = new Measurement();
            var updatedPrefix = $@"{prefix}/födelsevikt";
            this._prefix = updatedPrefix;
            this._occurance = occurance;

            Keyword = new KeywordCaseNote($@"{updatedPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    {base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
