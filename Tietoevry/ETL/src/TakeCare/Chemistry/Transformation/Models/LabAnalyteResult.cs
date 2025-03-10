namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class LabAnalyteResult
    {
        private readonly string _prefix;
        private readonly int v;

        public CodedText AnalyteName { get; set; }

        //public string AnalyteResultGenericCode { get; set; }
        //public string AnalyteResultGenericValue { get; set; }
        //public string AnalyteResultGenericTerminology { get; set; }

        public List<CodedText> AnalyteResultCodedText { get; set; }

        public List<string> AnalyteResultFreeText { get; set; }

        public List<MetricValue> AnalyteResultQuantity { get; set; }

        public List<string> AnalyteResultTime { get; set; }

        public List<Proportion> AnalyteResultProportion { get; set; }

        public Keyword AnalyteKeyword { get; set; }
        public string ReferenceRangeGuide { get; set; }
        public string ValidationTimeStamp { get; set; }
        public Identifier Specimen { get; set; }
        public List<string> Comments { get; set; }

        public LabAnalyteResult(string prefix, int counter)
        {
            _prefix = prefix;
            v = counter;
            AnalyteResultCodedText = new List<CodedText>();
            AnalyteResultFreeText = new List<string>();
            AnalyteResultQuantity = new List<MetricValue>();
            AnalyteResultTime = new List<string>();
            AnalyteResultProportion = new List<Proportion>();
            Comments = new List<string>();

        }
        public override string ToString()
        {
            var result = $@"";
            
            if (AnalyteName!=null && !string.IsNullOrWhiteSpace(AnalyteName.Code))
            {
                result += $@"
                ""{_prefix}/analysresultat:{v}/analysnamn|code"": ""{AnalyteName.Code}"",
                ""{_prefix}/analysresultat:{v}/analysnamn|value"": ""{AnalyteName.Value}"",
                ""{_prefix}/analysresultat:{v}/analysnamn|terminology"": ""{AnalyteName.Terminology}"",";
            }

            if (AnalyteResultCodedText!=null && AnalyteResultCodedText.Count>0)
            {
                for(int i = 0; i < AnalyteResultCodedText.Count; i++)
                {
                    result += $@"
                ""{_prefix}/analysresultat:{v}/analysresultat_text_med_kod:{i}|code"": ""{AnalyteResultCodedText[i].Code}"",
                ""{_prefix}/analysresultat:{v}/analysresultat_text_med_kod:{i}|value"": ""{AnalyteResultCodedText[i].Value}"",
                ""{_prefix}/analysresultat:{v}/analysresultat_text_med_kod:{i}|terminology"": ""{AnalyteResultCodedText[i].Terminology}"",";
                }
            }

            if (AnalyteResultFreeText!=null && AnalyteResultFreeText.Count>0)
            {
                for(int i = 0; i < AnalyteResultFreeText.Count; i++)
                {
                    result += $@"
                ""{_prefix}/analysresultat:{v}/analysresultat_fritext:{i}"": ""{AnalyteResultFreeText[i]}"",";
                }
            }

            if (AnalyteResultQuantity!=null && AnalyteResultQuantity.Count>0)
            {
                for(int i = 0; i < AnalyteResultQuantity.Count; i++)
                {
                    result += $@"
                ""{_prefix}/analysresultat:{v}/analysresultat_kvantitet:{i}|magnitude"": ""{AnalyteResultQuantity[i].Magnitude}"",
                ""{_prefix}/analysresultat:{v}/analysresultat_kvantitet:{i}|unit"": ""{AnalyteResultQuantity[i].Unit}"",
                ""{_prefix}/analysresultat:{v}/analysresultat_kvantitet:{i}|normal_status"": ""{AnalyteResultQuantity[i].NormalStatus}"",";
                }
            }

            if (AnalyteResultTime!=null && AnalyteResultTime.Count>0)
            {
                for(int i = 0; i < AnalyteResultTime.Count; i++)
                {
                    result += $@"
                ""{_prefix}/analysresultat:{v}/analysresultat_tid:{i}"": ""{AnalyteResultTime[i]}"",";
                }
            }

            if (AnalyteResultProportion!=null && AnalyteResultProportion.Count>0)
            {
                for(int i = 0; i < AnalyteResultProportion.Count; i++)
                {
                    result += $@"
                ""{_prefix}/analysresultat:{v}/analysresultat_proportion:{i}|numerator"": ""{AnalyteResultProportion[i].Numerator}"",
                ""{_prefix}/analysresultat:{v}/analysresultat_proportion:{i}|denominator"": ""{AnalyteResultProportion[i].Denominator}"",
                ""{_prefix}/analysresultat:{v}/analysresultat_proportion:{i}|type"": ""{AnalyteResultProportion[i].Type}"",
                ""{_prefix}/analysresultat:{v}/analysresultat_proportion:{i}"": ""{AnalyteResultProportion[i].Value}"",";
                }
            }

            if (AnalyteKeyword != null)
            {
                result += AnalyteKeyword.ToString();
            }

            if(!string.IsNullOrWhiteSpace(ReferenceRangeGuide))
            {
                result += $@"
                ""{_prefix}/analysresultat:{v}/vägledning_för_referensintervall"": ""{ReferenceRangeGuide}"",";
            }

            if(!string.IsNullOrWhiteSpace(ValidationTimeStamp))
            {
                result += $@"
                ""{_prefix}/analysresultat:{v}/tidpunkt_för_validering"": ""{ValidationTimeStamp}"",";
            }

            if(Specimen!=null)
            {
                result += $@"
                ""{_prefix}/analysresultat:{v}/provmaterial"": ""{Specimen.Id}"",
                ""{_prefix}/analysresultat:{v}/provmaterial|issuer"": ""{Specimen.Issuer}"",
                ""{_prefix}/analysresultat:{v}/provmaterial|assigner"": ""{Specimen.Assigner}"",
                ""{_prefix}/analysresultat:{v}/provmaterial|type"": ""{Specimen.Type}"",";
            }

            if(Comments!=null && Comments.Count>0)
            {
                for(int i = 0; i < Comments.Count; i++)
                {
                    result += $@"
                ""{_prefix}/analysresultat:{v}/kommentar:{i}"": ""{Comments[i]}"",";
                }
            }

            return result;
        }
    }
}