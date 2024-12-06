using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(6180, "TC Terminology")]
    public class TcSittingLength : Length
    {
        public KeywordCaseNote Keyword { get; set; }
        public TcLengthTerminology BodyPosition { get; set; }
        public TcLengthTerminology BodySegmentName { get; set; }
        public string Method { get; set; }
        public TcSittingLength(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
            this._occurance = occurance;

            LengthValue = new Measurement();
            BodyPosition = new TcLengthTerminology();
            BodySegmentName = new TcLengthTerminology();
            var keywordPrefix = $@"{prefix}/längd_sittande";
            this._prefix = keywordPrefix;
            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    ""{_prefix}:{_occurance}/_uid"": ""{Uid}"",
                    ""{_prefix}:{_occurance}/längd|magnitude"": ""{LengthValue.Magnitude}"",
                    ""{_prefix}:{_occurance}/längd|unit"": ""{LengthValue.Units}"",
                    ""{_prefix}:{_occurance}/benämning_på_kroppssegment|code"": ""{BodySegmentName.Code}"",
                    ""{_prefix}:{_occurance}/benämning_på_kroppssegment|value"": ""{BodySegmentName.Value}"",
                    ""{_prefix}:{_occurance}/benämning_på_kroppssegment|terminology"": ""{BodySegmentName.Terminology}"",
                    ""{_prefix}:{_occurance}/kroppsposition|code"": ""{BodyPosition.Code}"",
                    ""{_prefix}:{_occurance}/kroppsposition|value"": ""{BodyPosition.Value}"",
                    ""{_prefix}:{_occurance}/kroppsposition|terminology"": ""{BodyPosition.Terminology}"",
                    ""{_prefix}:{_occurance}/mätmetod"": ""{Method}"",
                    ""{_prefix}:{_occurance}/language|code"": ""{Language.Code}"",
                    ""{_prefix}:{_occurance}/language|terminology"": ""{Language.Value}"",
                    ""{_prefix}:{_occurance}/encoding|code"": ""{Encoding.Code}"",
                    ""{_prefix}:{_occurance}/encoding|terminology"": ""{Encoding.Value}"",
                    {Keyword.ToString()}";
        }
    }
}
