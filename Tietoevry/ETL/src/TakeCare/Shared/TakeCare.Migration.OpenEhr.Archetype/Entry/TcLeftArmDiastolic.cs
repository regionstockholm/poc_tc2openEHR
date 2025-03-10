using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Archetype.Entry
{
    [TypeInfo(6137, "TC Terminology")]
    public class TcLeftArmDiastolic : ArmDiastolic
    {
        private const string _termName = "blodtryck_vä_arm_diastoliskt";
        public KeywordCaseNote Keyword { get; set; }

        public TcLeftArmDiastolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            Diastolic = new Measurement();
            var updatedPrefix = $@"{prefix}/{_termName}";
            this._prefix = updatedPrefix;
            this._occurance = occurance;
            Keyword = new KeywordCaseNote($@"{updatedPrefix}:{occurance}");
            MeasurementLocation = new Terminology() { Code= "at0026", Type= "local", Value= "Vänster arm" };
        }

        public override string ToString()
        {
               return $@"{base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
