using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class AnyEvent
    {
        private readonly string _prefix;
        private readonly int v;

        public string Time { get; set; }
        public CodedText TestName { get; set; }
        public CodedText OverallTestStatus { get; set; }
        public string OverallTestStatusTimestamp { get; set; }
        public CodedText DiagnosticServiceCategory { get; set; }
        public string ClinicalInformationProvided { get; set; }
        public string Conclusion { get; set; }
        public List<string> TestDiagnosis { get; set; }
        public List<string> Comments { get; set; }
        //public List<string> CompoundingFactors { get; set; }
        //public List<Specimen> Specimen { get; set; }
        public List<LabAnalyteResult> AnalyteResult { get; set; }

        public AnyEvent(string prefix, int counter)
        {
            _prefix = prefix;
            v = counter;
            Comments = new List<string>();
            //CompoundingFactors = new List<string>();
            TestDiagnosis = new List<string>();
            //Specimen = new List<Specimen>();
            AnalyteResult = new List<LabAnalyteResult>();
        }
        public override string ToString()
        {
            var result = $@"";

            if (TestName!=null && !string.IsNullOrEmpty(TestName.Code))
            {
                result += $@"
                ""{_prefix}:{v}/undersökningsnamn|code"": ""{TestName.Code}"", 
                ""{_prefix}:{v}/undersökningsnamn|value"": ""{TestName.Value}"",                
                ""{_prefix}:{v}/undersökningsnamn|terminology"": ""{TestName.Terminology}"",";
            }

            if (!string.IsNullOrEmpty(OverallTestStatus.Code))
            {
                result += $@"
                ""{_prefix}:{v}/status_undersökningsresultat:0|code"": ""{OverallTestStatus.Code}"",
                ""{_prefix}:{v}/status_undersökningsresultat:0|value"": ""{OverallTestStatus.Value}"",
                ""{_prefix}:{v}/status_undersökningsresultat:0|terminology"": ""{OverallTestStatus.Terminology}"",";
            }

            if (!string.IsNullOrEmpty(OverallTestStatusTimestamp))
            {
                result += $@"
                ""{_prefix}:{v}/tidpunkt_för_status_undersökningsresultat"": ""{OverallTestStatusTimestamp}"",";
            }

            if (DiagnosticServiceCategory!=null && !string.IsNullOrEmpty(DiagnosticServiceCategory.Code))
            {
                result += $@"
                ""{_prefix}:{v}/laboratoriedisciplin|code"": ""{DiagnosticServiceCategory.Code}"",
                ""{_prefix}:{v}/laboratoriedisciplin|value"": ""{DiagnosticServiceCategory.Value}"",
                ""{_prefix}:{v}/laboratoriedisciplin|terminology"": ""{DiagnosticServiceCategory.Terminology}"",";
            }

            if (!string.IsNullOrEmpty(ClinicalInformationProvided))
            {
                result += $@"
                ""{_prefix}:{v}/tillgänglig_klinisk_information"": ""{ClinicalInformationProvided}"",";
            }

            if (!string.IsNullOrEmpty(Conclusion))
            {
                result += $@"
                ""{_prefix}:{v}/tolkning_av_resultat"": ""{Conclusion}"",";
            }

            /*
            if (TestDiagnosis!=null && TestDiagnosis.Count>0)
            {
                for (int i = 0 ; i < TestDiagnosis.Count; i++) {
                    result += $@"
                ""{_prefix}:{v}/undersökningsdiagnos:{i}"": ""{TestDiagnosis}"",";
                }
            }*/

            if (Comments!=null && Comments.Count>0)
            {
                for (int i = 0; i < Comments.Count; i++)
                {
                    result += $@"
                ""{_prefix}:{v}/kommentar:{i}"": ""{Comments[i]}"",";
                }
            }
            /*
            if (CompoundingFactors!=null && CompoundingFactors.Count>0)
            {
                for (int i = 0; i < CompoundingFactors.Count; i++)
                {
                    result += $@"
                ""{_prefix}:{v}/möjliga_felkällor:{i}"": ""{CompoundingFactors[i]}"",";
                }
            }*/

            /*
            if(Specimen!=null && Specimen.Count>0)
            {
                foreach(var specimen in Specimen)
                {
                    result += specimen.ToString();
                }
            }*/

            if(AnalyteResult!=null && AnalyteResult.Count>0)
            {
                foreach(var analyteResult in AnalyteResult)
                {
                    result += analyteResult.ToString();
                }
            }

            if (!string.IsNullOrWhiteSpace(Time)) {
                result += $@"
                ""{_prefix}:{v}/time"": ""{Time}"",";
            }

            return result;
        }
    }
}
