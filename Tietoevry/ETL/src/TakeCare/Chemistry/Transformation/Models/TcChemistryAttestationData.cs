using System;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class TcChemistryAttestationData
    {
        private readonly string _prefix;
        public List<Activity> Activities { get; set; }
        public string HasDeviatingAnalysisResults { get; set; }
        public string? AttestionCreatedTimestamp { get; set; }
        public string? ResponsibleAttesterName { get; set; }
        public string IsLatestVersionAttested { get; set; }
        public Attester  ResponsibleAttester { get; set; }

        public TcChemistryAttestationData(string prefix)
        {
            _prefix = prefix;
            Activities = new List<Activity>();
        }


        public override string ToString()
        {
            var result = $@"";

            if (HasDeviatingAnalysisResults != null)
            {
                result += $@"
                ""{_prefix}/har_avvikande_analysresultat"": {HasDeviatingAnalysisResults},";
            }
            if (!string.IsNullOrEmpty(AttestionCreatedTimestamp))
            {
                result += $@"
                ""{_prefix}/tidsstämpel_för_vidimering"": ""{AttestionCreatedTimestamp}"",";
            }

            if (!string.IsNullOrEmpty(ResponsibleAttesterName))
            {
                result += $@"
                ""{_prefix}/vidimeringsansvarig_namn"": ""{ResponsibleAttesterName}"",";
            }

            if (IsLatestVersionAttested!=null)
            {
                result += $@"
                ""{_prefix}/är_senaste_version_vidimerad"": {IsLatestVersionAttested},";
            }

            if (ResponsibleAttester != null)
            {
                result += $@"
                ""{_prefix}/ansvarig_vidimerare/aktivitet|code"": ""{ResponsibleAttester.Activity.ActivityData.Code}"",
                ""{_prefix}/ansvarig_vidimerare/aktivitet|value"": ""{ResponsibleAttester.Activity.ActivityData.Value}"",
                ""{_prefix}/ansvarig_vidimerare/aktivitet|terminology"": ""{ResponsibleAttester.Activity.ActivityData.Terminology}"",";

                if (ResponsibleAttester.CareUnit != null)
                {
                    result += $@"
                ""{_prefix}/ansvarig_vidimerare/tc_vårdenhet/id"": ""{ResponsibleAttester.CareUnit.Id}"",
                ""{_prefix}/ansvarig_vidimerare/tc_vårdenhet/id|issuer"": ""{ResponsibleAttester.CareUnit.Issuer}"",
                ""{_prefix}/ansvarig_vidimerare/tc_vårdenhet/id|assigner"": ""{ResponsibleAttester.CareUnit.Assigner}"",
                ""{_prefix}/ansvarig_vidimerare/tc_vårdenhet/id|type"": ""{ResponsibleAttester.CareUnit.Type}"",
                ""{_prefix}/ansvarig_vidimerare/tc_vårdenhet/namn"": ""{ResponsibleAttester.CareUnit.Name}"",";
                }
                if (ResponsibleAttester.User!=null)
                {
                    result += $@"
                ""{_prefix}/ansvarig_vidimerare/användare/id"": ""{ResponsibleAttester.User.Id}"",
                ""{_prefix}/ansvarig_vidimerare/användare/id|issuer"": ""{ResponsibleAttester.User.Issuer}"",
                ""{_prefix}/ansvarig_vidimerare/användare/id|assigner"": ""{ResponsibleAttester.User.Assigner}"",
                ""{_prefix}/ansvarig_vidimerare/användare/id|type"": ""{ResponsibleAttester.User.Type}"",
                ""{_prefix}/ansvarig_vidimerare/användare/fullständigt_namn"": ""{ResponsibleAttester.User.FullName}"",
                ""{_prefix}/ansvarig_vidimerare/användare/användarnamn"": ""{ResponsibleAttester.User.Username}"",";
                }
                
            }

            for(int i = 0; i < Activities.Count; i++)
            {
                if (!string.IsNullOrEmpty(Activities[i].ActivityData.Code)){
                    result += $@"
                ""{_prefix}/vidimerad:{i}/aktivitet|code"": ""{Activities[i].ActivityData.Code}"",
                ""{_prefix}/vidimerad:{i}/aktivitet|value"": ""{Activities[i].ActivityData.Value}"",
                ""{_prefix}/vidimerad:{i}/aktivitet|terminology"": ""{Activities[i].ActivityData.Terminology}"",";
                }
                if (!string.IsNullOrEmpty(Activities[i].ActivityTimestamp))
                {
                    result += $@"
                ""{_prefix}/vidimerad:{i}/tidsstämpel_för_aktivitet"": ""{Activities[i].ActivityTimestamp}"",";
                }
                if (Activities[i].User != null)
                {
                    result += $@"
                ""{_prefix}/vidimerad:{i}/användare/id"": ""{Activities[i].User.Id}"",
                ""{_prefix}/vidimerad:{i}/användare/id|issuer"": ""{Activities[i].User.Issuer}"",
                ""{_prefix}/vidimerad:{i}/användare/id|assigner"": ""{Activities[i].User.Assigner}"",
                ""{_prefix}/vidimerad:{i}/användare/id|type"": ""{Activities[i].User.Type}"",
                ""{_prefix}/vidimerad:{i}/användare/fullständigt_namn"": ""{Activities[i].User.FullName}"",
                ""{_prefix}/vidimerad:{i}/användare/användarnamn"": ""{Activities[i].User.Username}"",";

                }
            }

            return result;

        }
    }

    public class Attester
    {
        public Activity Activity { get; set; }
        public User User { get; set; }
        public CareUnitDetails CareUnit  { get; set; }
    }

    public class Activity
    {
        public CodedText ActivityData { get; set; }
        public string? ActivityTimestamp { get; set; }

        public User User { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Issuer { get; set; }
        public string Assigner { get; set; }
        public string Type { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public Profession Profession { get; set; }
    }

    public class Profession
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string Terminology { get; set; }
    }

}