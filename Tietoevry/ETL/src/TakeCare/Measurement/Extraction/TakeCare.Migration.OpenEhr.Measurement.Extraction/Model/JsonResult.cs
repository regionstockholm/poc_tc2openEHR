namespace TakeCare.Migration.OpenEhr.Measurement.Extraction.Model
{
    public class JsonResult
    {
        public string PatientId { get; set; }
        public string Attestation { get; set; }
        public string Attester { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string Id { get; set; }
        public string LinkCode { get; set; }
        public string VersionId { get; set; }
        public UserDetails Created { get; set; }
        public bool IsCancelled { get; set; }
        public UserDetails Saved { get; set; }
        public List<Measurement> Measurements { get; set; }
        public Template Template { get; set; }
    }
}
