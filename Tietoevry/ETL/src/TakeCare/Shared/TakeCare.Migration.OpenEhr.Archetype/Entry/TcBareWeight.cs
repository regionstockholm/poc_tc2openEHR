﻿using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(5028, "TC Terminology")]
    public class TcBareWeight : Weight
    {
        public KeywordCaseNote Keyword { get; set; }

        public TcBareWeight(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            
            WeightValue = new Measurement();
            var updatedPrefix = $@"{prefix}/nakenvikt";
            this._prefix = updatedPrefix;
            this._occurance = occurance;

            Keyword = new KeywordCaseNote($@"{updatedPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    {base.ToString()}
                    ""vårdkontakt/ickm/nakenvikt:0/klädsel|code"": ""at0013"",
                    ""vårdkontakt/ickm/nakenvikt:0/klädsel|value"": ""Naken"",
                    ""vårdkontakt/ickm/nakenvikt:0/klädsel|terminology"": ""local"",
                    {Keyword.ToString()}";
        }
    }
}
