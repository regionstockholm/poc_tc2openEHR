namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class TcServiceRequest
    {
        private readonly string _prefix;
        public string Uid { get; set; }
        public string RequesterOrderIdentifier { get; set; }
        public string ExpiryTime { get; set; }
        public string Narrative { get; set; }
        public CurrentActivity CurrentActivity { get; set; }

        public TcServiceRequest(string prefix)
        {
            _prefix = $"{prefix}/förfrågan_om_hälso-och_sjukvårdsrelaterad_tjänst";
            CurrentActivity = new CurrentActivity(_prefix);
        }

        public override string ToString()
        {
            var result = $@"";

            if (!string.IsNullOrWhiteSpace(Uid))
            {
                result += $@"
                ""{_prefix}/_uid"": ""{Uid}"",";
            }

            if (!string.IsNullOrWhiteSpace(Narrative))
            {
                result += $@"
                ""{_prefix}/narrative"": ""{Narrative}"",";
            }

            if (CurrentActivity != null)
            {
                result += CurrentActivity.ToString();
            }

            return result;
        }
    }
}
