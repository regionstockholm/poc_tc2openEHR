using TakeCare.Foundation.OpenEhr.Application.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class ServiceDirection
    {
        private readonly string _prefix;
        public List<ServiceActivity> Activities { get; set; }

        public PeriodicActivity PeriodicActivities { get; set; }

        public ServiceDirection(string prefix)
        {
            _prefix = prefix;
            Activities = new List<ServiceActivity>();
        }

        public override string ToString()
        {
            var result = $@"";
            for (int i = 0; i < Activities.Count; i++)
            {
                for(int j = 0; j < Activities[i].SpecificTimes.Count; j++)
                {
                    result += $@"
                ""{_prefix}/tjänstriktning:0/aktivitet:0/timing_-_dagligen:{i}/tidpunkt:{j}"": ""{Activities[i].SpecificTimes[j]}"",";

                }
            }

            if(PeriodicActivities != null)
            {
                if (!string.IsNullOrWhiteSpace(PeriodicActivities.RecurringInterval))
                {
                    result += $@"
                ""{_prefix}/tjänstriktning:0/timing_icke-daglig/upprepningsintervall"": ""{PeriodicActivities.RecurringInterval}"",";

                }
            }

            return result;
        }
    }

    public class PeriodicActivity
    {
        public string RecurringInterval { get; set; }
    }

    public class ServiceActivity
    {
        public List<string> SpecificTimes { get; set; }

        public ServiceActivity()
        {
            SpecificTimes = new List<string>();
        }
    }

}
