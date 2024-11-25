namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class TcActivityMetadata
    {
        private readonly string _prefix;

        public string? DocumentId { get; set; }
        public ActivityUser? DocCreatedByUser { get; set; }
        public ActivityUser? DocSavedByUser { get; set; }
        public ActivityUser? LockInfo { get; set; }
        public ActivityUser? Signer { get; set; }
        public ActivityUser? Signed { get; set; }

        public TcActivityMetadata(string prefix)
        {
            _prefix = prefix;
        }

        public override string ToString()
        {
            var result = $@"";

            if (!string.IsNullOrWhiteSpace(DocumentId))
            {
                result += $@"
                ""{_prefix}/dokument_id"": ""{DocumentId}"",";
            }

            if (DocCreatedByUser != null)
            {
                if (DocCreatedByUser.ActivityCode != null)
                {
                    result += $@"
                ""{_prefix}/skapat/aktivitet|code"": ""{DocCreatedByUser.ActivityCode.Code}"",
                ""{_prefix}/skapat/aktivitet|value"": ""{DocCreatedByUser.ActivityCode.Value}"",
                ""{_prefix}/skapat/aktivitet|terminology"": ""{DocCreatedByUser.ActivityCode.Terminology}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocCreatedByUser.ActivityTimestamp))
                {
                    result += $@"
                ""{_prefix}/skapat/tidsstämpel_för_aktivitet"": ""{DocCreatedByUser.ActivityTimestamp}"",";
                }

                if (DocCreatedByUser.UserId != null)
                {
                    result += $@"
                ""{_prefix}/skapat/användare/id"": ""{DocCreatedByUser.UserId.Value}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocCreatedByUser.FullName))
                {
                    result += $@"
                ""{_prefix}/skapat/användare/fullständigt_namn"": ""{DocCreatedByUser.FullName}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocCreatedByUser.Username))
                {
                    result += $@"
                ""{_prefix}/skapat/användare/användarnamn"": ""{DocCreatedByUser.Username}"",";
                }

                if (DocCreatedByUser.CareUnitId != null)
                {
                    result += $@"
                ""{_prefix}/skapat/tc_vårdenhet/id"": ""{DocCreatedByUser.CareUnitId.Value}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocCreatedByUser.CareUnitName))
                {
                    result += $@"
                ""{_prefix}/skapat/tc_vårdenhet/namn"": ""{DocCreatedByUser.CareUnitName}"",";
                }
            }

            if (DocSavedByUser != null)
            {
                if (DocSavedByUser.ActivityCode != null)
                {
                    result += $@"
                ""{_prefix}/sparad/aktivitet|code"": ""{DocSavedByUser.ActivityCode.Code}"",
                ""{_prefix}/sparad/aktivitet|value"": ""{DocSavedByUser.ActivityCode.Value}"",
                ""{_prefix}/sparad/aktivitet|terminology"": ""{DocSavedByUser.ActivityCode.Terminology}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocSavedByUser.ActivityTimestamp))
                {
                    result += $@"
                ""{_prefix}/sparad/tidsstämpel_för_aktivitet"": ""{DocSavedByUser.ActivityTimestamp}"",";
                }

                if (DocSavedByUser.UserId != null)
                {
                    result += $@"
                ""{_prefix}/sparad/användare/id"": ""{DocSavedByUser.UserId.Value}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocSavedByUser.FullName))
                {
                    result += $@"
                ""{_prefix}/sparad/användare/fullständigt_namn"": ""{DocSavedByUser.FullName}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocSavedByUser.Username))
                {
                    result += $@"
                ""{_prefix}/sparad/användare/användarnamn"": ""{DocSavedByUser.Username}"",";
                }

                if (DocSavedByUser.CareUnitId != null)
                {
                    result += $@"
                ""{_prefix}/sparad/tc_vårdenhet/id"": ""{DocSavedByUser.CareUnitId.Value}"",";
                }

                if (!string.IsNullOrWhiteSpace(DocSavedByUser.CareUnitName))
                {
                    result += $@"
                ""{_prefix}/sparad/tc_vårdenhet/namn"": ""{DocSavedByUser.CareUnitName}"",";
                }
            }

            if (LockInfo != null)
            {
                if (LockInfo.ActivityCode!=null)
                {
                    result += $@"
                ""{_prefix}/låst/aktivitet|code"": ""{LockInfo.ActivityCode.Code}"",
                ""{_prefix}/låst/aktivitet|value"": ""{LockInfo.ActivityCode.Value}"",
                ""{_prefix}/låst/aktivitet|terminology"": ""{LockInfo.ActivityCode.Terminology}"",";
                }

                if (!string.IsNullOrWhiteSpace(LockInfo.ActivityTimestamp))
                {
                    result += $@"
                ""{_prefix}/låst/tidsstämpel_för_aktivitet"": ""{LockInfo.ActivityTimestamp}"",";
                }

                if (LockInfo.UserId != null)
                {
                    result += $@"
                ""{_prefix}/låst/användare/id"": ""{LockInfo.UserId.Value}"",";
                }

                if (!string.IsNullOrWhiteSpace(LockInfo.FullName))
                {
                    result += $@"
                ""{_prefix}/låst/användare/fullständigt_namn"": ""{LockInfo.FullName}"",";
                }

                if (!string.IsNullOrWhiteSpace(LockInfo.Username))
                {
                    result += $@"
                ""{_prefix}/låst/användare/användarnamn"": ""{LockInfo.Username}"",";
                }

            }

            if (Signed != null)
            {
                if (Signed.ActivityCode != null)
                {
                    result += $@"
                ""{_prefix}/signerad/aktivitet|code"": ""{Signed.ActivityCode.Code}"",
                ""{_prefix}/signerad/aktivitet|value"": ""{Signed.ActivityCode.Value}"",
                ""{_prefix}/signerad/aktivitet|terminology"": ""{Signed.ActivityCode.Terminology}"",";
                }

                if (!string.IsNullOrWhiteSpace(Signed.ActivityTimestamp))
                {
                    result += $@"
                ""{_prefix}/signerad/tidsstämpel_för_aktivitet"": ""{Signed.ActivityTimestamp}"",";
                }

                if (Signed.UserId != null)
                {
                    result += $@"
                ""{_prefix}/signerad/användare/id"": ""{Signed.UserId.Value}"",";
                }

                if (!string.IsNullOrWhiteSpace(Signed.FullName))
                {
                    result += $@"
                ""{_prefix}/signerad/användare/fullständigt_namn"": ""{Signed.FullName}"",";
                }

                if (!string.IsNullOrWhiteSpace(Signed.Username))
                {
                    result += $@"
                ""{_prefix}/signerad/användare/användarnamn"": ""{Signed.Username}"",";
                }
            }

            if (Signer != null)
            {
                if (Signer.ActivityCode != null)
                {
                    result += $@"
                ""{_prefix}/signerat_av/aktivitet|code"": ""{Signer.ActivityCode.Code}"",
                ""{_prefix}/signerat_av/aktivitet|value"": ""{Signer.ActivityCode.Value}"",
                ""{_prefix}/signerat_av/aktivitet|terminology"": ""{Signer.ActivityCode.Terminology}"",";
                }

                if (!string.IsNullOrWhiteSpace(Signer.ActivityTimestamp))
                {
                    result += $@"
                ""{_prefix}/signerat_av/tidsstämpel_för_aktivitet"": ""{Signer.ActivityTimestamp}"",";
                }

                if (Signer.UserId != null)
                {
                    result += $@"
                ""{_prefix}/signerat_av/användare/id"": ""{Signer.UserId.Value}"",";
                }

                if (!string.IsNullOrWhiteSpace(Signer.FullName))
                {
                    result += $@"
                ""{_prefix}/signerat_av/användare/fullständigt_namn"": ""{Signer.FullName}"",";
                }

                if (!string.IsNullOrWhiteSpace(Signer.Username))
                {
                    result += $@"
                ""{_prefix}/signerat_av/användare/användarnamn"": ""{Signer.Username}"",";
                }
            }

            return result;

        }

    }

    public class ActivityUser
    {
        public CodedText? ActivityCode { get; set; }
        public string? ActivityTimestamp { get; set; }
        public Identifier? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public Identifier? CareUnitId { get; set; }
        public string? CareUnitName { get; set; }
    }
}
