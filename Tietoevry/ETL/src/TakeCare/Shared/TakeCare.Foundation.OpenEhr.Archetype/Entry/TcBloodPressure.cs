using Spine.Foundation.Web.OpenEhr.Archetype.Entry;

namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcBloodPressure : BloodPressure
    {
       // bloodtrack
        public Keyword Keyword { get; set; }

        public TcBloodPressure(string prefix, string occurance) : base(prefix, occurance)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(occurance);
            _occurance = occurance;
            Systolic = new Measurement();
            Diastolic = new Measurement();
            var keywordPrefix = $@"{prefix}/blodtryck";
            Keyword = new Keyword($@"{keywordPrefix}:{occurance}");
        }

        public override string ToString()
        {
            return $@"
                    {base.ToString()}
                    {Keyword.ToString()}";
        }
    }

}
