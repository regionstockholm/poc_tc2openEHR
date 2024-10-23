using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcRightArmDiastolic : ArmDiastolic
    {
        private const string _termName = "blodtryck_hö_arm_diastoliskt";
        public Keyword Keyword { get; set; }

        public TcRightArmDiastolic(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            Diastolic = new Measurement();
            var updatedPrefix = $@"{prefix}/{_termName}";
            this._prefix = updatedPrefix;
            this._occurance = occurance;
            Keyword = new Keyword($@"{updatedPrefix}:{occurance}");
            MeasurementLocation = new Terminology() { Code = "at0025", Type = "local", Value = "Höger arm" };
        }

        public override string ToString()
        {
               return $@"{base.ToString()}
                    {Keyword.ToString()}";
        }
    }
}
