namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class CurrentActivity
    {
        private readonly string _prefix;

        public CodedText ServiceName { get; set; }
        public CodedText ServiceType { get; set; }
        public string Description { get; set; }
        public CodedText Urgency { get; set; }
        public ServiceDue ServiceDue { get; set; }
        public string Timing { get; set; }
        public string TimingFormalism { get; set; }
        public ActivityExtension Extension { get; set; }
        public ServiceDirection ServiceDirection { get; set; }
        public string ServicePeriodStart { get; set; }
        public string ServicePeriodExpiry { get; set; }

        public CurrentActivity(string prefix)
        {            
            _prefix = $@"{prefix}/nuvarande_aktivitet";
            Extension = new ActivityExtension(_prefix);
            ServiceDirection = new ServiceDirection(_prefix);
        }
        
        public override string ToString()
        {
            var result = $@"";
            
            if (ServiceName != null)
            {
                result += $@"
                ""{_prefix}/namn_på_tjänst|code"": ""{ServiceName.Code}"",
                ""{_prefix}/namn_på_tjänst|value"": ""{ServiceName.Value}"",
                ""{_prefix}/namn_på_tjänst|terminology"": ""{ServiceName.Terminology}"",";
            }
            
            if (ServiceType != null)
            {
                result += $@"
                ""{_prefix}/typ_av_tjänst|code"": ""{ServiceType.Code}"",
                ""{_prefix}/typ_av_tjänst|value"": ""{ServiceType.Value}"",
                ""{_prefix}/typ_av_tjänst|terminology"": ""{ServiceType.Terminology}"",";
            }
            
            if (!string.IsNullOrWhiteSpace(Description))
            {
                result += $@"
                ""{_prefix}/beskrivning"": ""{Description}"",";
            }
            
            if (Urgency!=null)
            {
                result += $@"
                ""{_prefix}/brådskandegrad|code"": {Urgency.Code},
                ""{_prefix}/brådskandegrad|value"": ""{Urgency.Value}"",
                ""{_prefix}/brådskandegrad|terminology"": ""{Urgency.Terminology}"",";
            }
            
            if (ServiceDue != null)
            {
                if (!string.IsNullOrWhiteSpace(ServiceDue.Lower))
                {
                    result += $@"
                ""{_prefix}/datum_tid_förfall/interval_of_date_time_value/lower"": ""{ServiceDue.Lower}"",";
                }
                if (!string.IsNullOrWhiteSpace(ServiceDue.Upper))
                {
                    result += $@"
                ""{_prefix}/datum_tid_förfall/interval_of_date_time_value/upper"": ""{ServiceDue.Upper}"",";
                }
                if (!string.IsNullOrWhiteSpace(ServiceDue.LowerUnbounded))
                {
                    result += $@"
                ""{_prefix}/datum_tid_förfall/interval_of_date_time_value|lower_unbounded"": {ServiceDue.LowerUnbounded},";
                }
                if (!string.IsNullOrWhiteSpace(ServiceDue.UpperUnbounded))
                {
                    result += $@"
                ""{_prefix}/datum_tid_förfall/interval_of_date_time_value|upper_unbounded"": {ServiceDue.UpperUnbounded},";
                }
                if (!string.IsNullOrWhiteSpace(ServiceDue.DateTimeValue))
                {
                    result += $@"
                ""{_prefix}/datum_tid_förfall/date_time_value"": ""{ServiceDue.DateTimeValue}"",";
                }
            }

            if(!string.IsNullOrWhiteSpace(ServicePeriodStart))
            {
                result += $@"
                ""{_prefix}/tjänsteperiodens_start"": ""{ServicePeriodStart}"",";
            }

            if(!string.IsNullOrWhiteSpace(ServicePeriodExpiry))
            {
                result += $@"
                ""{_prefix}/tjänsteperiodens_slut"": ""{ServicePeriodExpiry}"",";
            }

            if (ServiceDirection != null)
            {
                result += ServiceDirection.ToString();
            }

            if (Extension != null)
            {
                result += Extension.ToString();
            }

            if (!string.IsNullOrWhiteSpace(Timing))
            {
                result += $@"
                ""{_prefix}/timing"": ""{Timing}"",
                ""{_prefix}/timing|formalism"": ""{TimingFormalism}"",";
            }
            
            return result;
        }
    }
}
