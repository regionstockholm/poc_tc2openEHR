namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class ActivityExtension
    {
        private readonly string _prefix;
        public string BasedOnActivityId { get; set; }
        public string DateTimeCompletedSetBySystem { get; set; }
        public string DateTimeCompletedSetByUser { get; set; }
        public Identifier LinkedDocumentId { get; set; }
        public Identifier LinkedDocumentTypeId { get; set; }
        public string LinkedDocumentTypeName { get; set; }
        public int? PriorityValue { get; set; }
        public CodedText Status { get; set; }
        public List<string> PlannedAt { get; set; }
        public List<string> IrregularDateTimes { get; set; }
        public Profession Profession { get; set; }

        public ActivityExtension(string prefix)
        {
            _prefix = $"{prefix}/aktivitet_extension";
            PlannedAt = new List<string>();
            IrregularDateTimes = new List<string>();
        }

        public override string ToString()
        {
            var result = $@"";
            if (!string.IsNullOrWhiteSpace(BasedOnActivityId))
            {
                result += $@"
                ""{_prefix}/baserad_på_aktivitets_id"": ""{BasedOnActivityId}"",";
            }
            if (!string.IsNullOrWhiteSpace(DateTimeCompletedSetBySystem))
            {
                result += $@"
                ""{_prefix}/datum_tidpunkt_utförd_registrerat_av_systemet"": ""{DateTimeCompletedSetBySystem}"",";
            }
            if (!string.IsNullOrWhiteSpace(DateTimeCompletedSetByUser))
            {
                result += $@"
                ""{_prefix}/datum_tidpunkt_utförd_registrerat_av_användaren"": ""{DateTimeCompletedSetByUser}"",";
            }
            if (LinkedDocumentId != null)
            {
                result += $@"
                ""{_prefix}/länkat_dokument_id"": {LinkedDocumentId.Value},";
            }
            if (LinkedDocumentTypeId != null)
            {
                result += $@"
                ""{_prefix}/länkad_dokument_typ_id"": {LinkedDocumentTypeId.Value},";
            }
            if (!string.IsNullOrWhiteSpace(LinkedDocumentTypeName))
            {
                result += $@"
                ""{_prefix}/länkad_dokument_typ_namn"": ""{LinkedDocumentTypeName}"",";
            }
            if (PriorityValue != null)
            {
                result += $@"
                ""{_prefix}/prioritets_värde"": ""{PriorityValue}"",";
            }
            if (Status != null)
            {
                result += $@"
                ""{_prefix}/status|code"": ""{Status.Code}"",
                ""{_prefix}/status|value"": ""{Status.Value}"",
                ""{_prefix}/status|terminology"": ""{Status.Terminology}"",";
            }
            if (PlannedAt != null && PlannedAt.Count > 0)
            {
                for (int i = 0; i < PlannedAt.Count; i++)
                {
                    result += $@"
                ""{_prefix}/planerad_tidpunkt:{i}"": ""{PlannedAt[i]}"",";
                }
            }
            if (Profession != null)
            {
                result += $@"
                ""{_prefix}/yrkes_id"": ""{Profession.Id}"",
                ""{_prefix}/yrke_kortnamn"": ""{Profession.ShortName}"",
                ""{_prefix}/yrkes_singularnamn"": ""{Profession.SingularName}"",";
            }
            if(IrregularDateTimes!=null && IrregularDateTimes.Count > 0)
            {
                for(int i = 0; i < IrregularDateTimes.Count; i++)
                {
                    result += $@"
                ""{_prefix}/oregelbundet_datumtid:{i}"": ""{IrregularDateTimes[i]}"",";
                }
            }
            return result;
        }
    }

    public class Profession
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string SingularName { get; set; }
    }

}