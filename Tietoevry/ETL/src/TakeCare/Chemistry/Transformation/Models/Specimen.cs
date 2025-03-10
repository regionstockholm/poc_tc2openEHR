using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class Specimen
    {
        private readonly int v;
        private readonly string _prefix;

        public string SpecimenTypeCode { get; set; }
        public string SpecimenTypeValue { get; set; }
        public string SpecimenTypeTerminology { get; set; }
        public string JunoSampleIdentifier { get; set; }
        public string CollectionTimestamp { get; set; }
        public List<string> SpecimenQualityIssue { get; set; }
        public string AdequacyForTesting { get; set; }
        public string Comment { get; set; }

        public Specimen(string prefix, int counter)
        {
            v = counter;
            _prefix = prefix;
            SpecimenQualityIssue = new List<string>();
        }
        public override string ToString()
        {
            var result = $@"";

            if (!string.IsNullOrWhiteSpace(SpecimenTypeCode))
            {
                result += $@"
                ""{_prefix}/provmaterial:{v}/provtyp|code"": ""{SpecimenTypeCode}"",
                ""{_prefix}/provmaterial:{v}/provtyp|value"": ""{SpecimenTypeValue}"",
                ""{_prefix}/provmaterial:{v}/provtyp|terminology"": ""{SpecimenTypeTerminology}"",";
            }

            if (!string.IsNullOrWhiteSpace(JunoSampleIdentifier))
            {
                result += $@"
                ""{_prefix}/provmaterial:{v}/extern_identifierare"": ""{JunoSampleIdentifier}"",";
            }

            if (!string.IsNullOrWhiteSpace(CollectionTimestamp))
            {
                result += $@"
                ""{_prefix}/provmaterial:{v}/datum_tid_för_provtagning"": ""{CollectionTimestamp}"",";
            }

            if (SpecimenQualityIssue != null)
            {
                for (int i = 0; i < SpecimenQualityIssue.Count; i++)
                {
                    result += $@"
                ""{_prefix}/provmaterial:{v}/provkvalitetsproblem|{i}"": ""{SpecimenQualityIssue[i]}"",";
                }
            }

            if (!string.IsNullOrWhiteSpace(AdequacyForTesting))
            {
                result += $@"
                ""{_prefix}/provmaterial:{v}/analysbarhet|code"": ""{AdequacyForTesting}"",";
            }

            if (!string.IsNullOrWhiteSpace(Comment))
            {
                result += $@"
                ""{_prefix}/provmaterial:{v}/kommentar"": ""{Comment}"",";
            }

            return result;
        }
    }
}
