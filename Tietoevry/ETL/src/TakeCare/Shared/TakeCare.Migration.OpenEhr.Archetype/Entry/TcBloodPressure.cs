using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(1849, "TC Terminology")]
    public class TcBloodPressure : BloodPressure
    {
       // bloodtrack
        public KeywordCaseNote Keyword { get; set; }

        public TcBloodPressure(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            _occurance = occurance;
            Systolic = new Measurement();
            Diastolic = new Measurement();
            var keywordPrefix = $@"{prefix}/blodtryck";
            Keyword = new KeywordCaseNote($@"{keywordPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    {base.ToString()}
                    {Keyword.ToString()}";
        }
    }

}
