using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(6136, "TC Terminology")]
    public class TcLeftArmSystolic : ArmSystolic
    {
        private const string _termName = "blodtryck_vä_arm_systoliskt";
        public KeywordCaseNote Keyword { get; set; }

        public TcLeftArmSystolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            Systolic = new Measurement();
            var keywordPrefix = $@"{prefix}/{_termName}:{occurance}";
            this._prefix = keywordPrefix;
            this._occurance = occurance;
            Keyword = new KeywordCaseNote(keywordPrefix);
            MeasurementLocation = new Terminology() { Code = "at0026", Type = "local", Value = "Vänster arm" };
        }

        public override string ToString()
        {
            return $@"{base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
