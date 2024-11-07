using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcRightArmSystolic : ArmSystolic
    {
        private const string _termName = "blodtryck_hö_arm_systoliskt";
        
        public KeywordCaseNote Keyword { get; set; }

        public TcRightArmSystolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            Systolic = new Measurement();
            var keywordPrefix = $@"{prefix}/{_termName}:{occurance}";
            this._prefix = keywordPrefix;
            this._occurance = occurance;
            Keyword = new KeywordCaseNote(keywordPrefix);
            MeasurementLocation = new Terminology() { Code = "at0025", Type = "local", Value = "Höger arm" };
        }

        public override string ToString()
        {
            return $@"{base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
