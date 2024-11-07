using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcHorizontalLength : Length
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcLengthTerminology BodyPosition { get; set; }

        public TcHorizontalLength(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;

            LengthValue = new Measurement();
            BodyPosition = new TcLengthTerminology();
            var keywordPrefix = $@"{prefix}/längd_liggande";
            this._prefix = keywordPrefix;

            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"{base.ToString()}
                    ""{_prefix}:{_occurance}/kroppsställning|code"": ""{BodyPosition.Code}"",
                    ""{_prefix}:{_occurance}/kroppsställning|value"": ""{BodyPosition.Value}"",
                    ""{_prefix}:{_occurance}/kroppsställning|terminology"": ""{BodyPosition.Terminology}"",
                    {Keyword.ToString()}";
        }
    }
}
