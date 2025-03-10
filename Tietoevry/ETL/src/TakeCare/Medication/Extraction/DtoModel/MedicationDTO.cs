namespace TakeCare.Migration.OpenEhr.Medication.Extraction.DtoModel
{
    public class MedicationDTO
    {
        public string Time { get; set; }

        public string User { get; set; }

        public string CareUnitIdType { get; set; }

        public string CareUnitId { get; set; }

        public string PatientId { get; set; }
        public List<TakeCare.Migration.OpenEhr.Medication.Extraction.Model.Medication> Medications { get; set; }
    }
}
