namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcCaseNoteContextMetadata
    {
        public string _prefix { get; set; }
        public string DocumentId { get; set; }

        public User DocCreatedByUser { get; set; }

        public Profession DocCreatedByProfession { get; set; }

        public Profession DocSavedByProfession { get; set; }

        public string DocCreatedAtCareUnitId { get; set; }

        public string DocCreatedTimestamp { get; set; }

        public User DocSavedByUSer { get; set; }

        public string DocSavedTimestamp { get; set; }

        public User Signer { get; set; }

        public User CounterSigner { get; set; }

        public User SignedBy { get; set; }

        public string SignedTimestamp { get; set; }

        public string ApprovedForPatient { get; set; }

        public string EventDateTime { get; set; }

        public string DocumentTitle { get; set; }

        public string TemplateName { get; set; }

        public string TemplateId { get; set; }

        public string DocumentCode { get; set; }

        public string HeaderTerm { get; set; }

        public TcCaseNoteContextMetadata(string prefix)
        {
            _prefix = $@"{prefix}/context/metadata";
        }

        public override string ToString()
        {
            var result = $@"
                            ""{_prefix}/dokument_id"": ""{DocumentId}"",
                            ""{_prefix}/dokumentationskod"": ""{DocumentCode}"",
                            ""{_prefix}/dokumentationstidpunkt"": ""{DocCreatedTimestamp}"",
                            ""{_prefix}/dokumentnamn"": ""{DocumentTitle}"",
                            ""{_prefix}/rubriktext"": ""{HeaderTerm}"",
                            ""{_prefix}/tidsstämpel_för_sparat_dokument"": ""{DocSavedTimestamp}"",
                            ""{_prefix}/dokumentationsmall|code"": ""{TemplateId}"",
                            ""{_prefix}/dokumentationsmall|value"": ""{TemplateName}"",
                            ""{_prefix}/dokumentationsmall|terminology"": ""TC-Template-codes"",";

            if (!string.IsNullOrEmpty(ApprovedForPatient))
            {
               result += $@"
                            ""{_prefix}/godkänd_för_patient"": ""{ApprovedForPatient}"",";
            }
            if (!string.IsNullOrEmpty(SignedTimestamp))
            {
               result += $@"
                            ""{_prefix}/signeringstidpunkt"": ""{SignedTimestamp}"",";
            }

            if (DocCreatedByProfession != null && !string.IsNullOrEmpty(DocCreatedByProfession.Code))
            {
               result += $@"
                            ""{_prefix}/dokument_skapad_av_yrkestitel_id|code"": ""{DocCreatedByProfession.Code}"",
                            ""{_prefix}/dokument_skapad_av_yrkestitel_id|value"": ""{DocCreatedByProfession.Value}"",
                            ""{_prefix}/dokument_skapad_av_yrkestitel_id|terminology"": ""{DocCreatedByProfession.Terminology}"",";
            }

            /*
            if (DocSavedByProfession != null && !string.IsNullOrEmpty(DocSavedByProfession.Code))
            {
                result += $@"""{_prefix}/dokument_sparat_av_yrkesroll_id|code"": ""{DocSavedByProfession.Code}"",
                             ""{_prefix}/dokument_sparat_av_yrkesroll_id|value"": ""{DocSavedByProfession.Value}"",
                             ""{_prefix}/dokument_sparat_av_yrkesroll_id|terminology"": ""{DocSavedByProfession.Terminology}"",";
            }
            */

            if (DocSavedByUSer != null && !string.IsNullOrEmpty(DocSavedByUSer.Id))
            {
               result += $@"
                            ""{_prefix}/dokument_sparat_av_användar_id"": ""{DocSavedByUSer.Id}"",
                            ""{_prefix}/dokument_sparat_av_användar_id|issuer"": ""{DocSavedByUSer.Issuer}"",
                            ""{_prefix}/dokument_sparat_av_användar_id|assigner"": ""{DocSavedByUSer.Assigner}"",
                            ""{_prefix}/dokument_sparat_av_användar_id|type"": ""{DocSavedByUSer.Type}"",
                            ""{_prefix}/dokument_sparat_av_fullständigt_namn"": ""{DocSavedByUSer.Name}"",";
            }

            if (DocCreatedByUser != null && !string.IsNullOrEmpty(DocCreatedByUser.Id))
            {
               result += $@"
                            ""{_prefix}/dokumentskaparens_användar_id"": ""{DocCreatedByUser.Id}"",
                            ""{_prefix}/dokumentskaparens_användar_id|issuer"": ""{DocCreatedByUser.Issuer}"",
                            ""{_prefix}/dokumentskaparens_användar_id|assigner"": ""{DocCreatedByUser.Assigner}"",
                            ""{_prefix}/dokumentskaparens_användar_id|type"": ""{DocCreatedByUser.Type}"",
                            ""{_prefix}/dokument_skapat_av_fullständigt_namn"": ""{DocCreatedByUser.Name}"",";
            }

            if (Signer != null && !string.IsNullOrEmpty(Signer.Id))
            {
               result += $@"
                            ""{_prefix}/signerare_id"": ""{Signer.Id}"",
                            ""{_prefix}/signerare_id|issuer"": ""{Signer.Issuer}"",
                            ""{_prefix}/signerare_id|assigner"": ""{Signer.Assigner}"",
                            ""{_prefix}/signerare_id|type"": ""{Signer.Type}"",";
            }
            if (SignedBy!=null && !string.IsNullOrEmpty(SignedBy.Id))
            {
               result += $@"
                            ""{_prefix}/signerat_av_id"": ""{SignedBy.Id}"",
                            ""{_prefix}/signerat_av_id|issuer"": ""{SignedBy.Issuer}"",
                            ""{_prefix}/signerat_av_id|assigner"": ""{SignedBy.Assigner}"",
                            ""{_prefix}/signerat_av_id|type"": ""{SignedBy.Type}"",";
            }
            if (CounterSigner!=null && !string.IsNullOrEmpty(CounterSigner.Id))
            {
               result += $@"
                            ""{_prefix}/kontrasignerare_id"": ""{CounterSigner.Id}"",
                            ""{_prefix}/kontrasignerare_id|issuer"": ""{CounterSigner.Issuer}"",
                            ""{_prefix}/kontrasignerare_id|assigner"": ""{CounterSigner.Assigner}"",
                            ""{_prefix}/kontrasignerare_id|type"": ""{CounterSigner.Type}"",";
            }
            return result;

        }
    }

    public class Profession
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string Terminology { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Issuer { get; set; }
        public string Assigner { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }

}
