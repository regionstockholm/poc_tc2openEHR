namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class TcAdministration
    {
        public Guid ParentGuid { get; set; }
        public Guid Guid { get; set; }
        public string Row { get; set; }
        public string TimestampSaved { get; set; }
        public string SavedByUserID { get; set; }
        public string SavedAtCareUnitID { get; set; }
        public string AsNeededNo { get; set; }
        public string TreatmentReason { get; set; }
        public string AdministrationDatetime { get; set; }
        public string TotalAmount { get; set; }
        public string Comment { get; set; }
        public string PrescriptionDate { get; set; }
        public string PrescriptionTime { get; set; }
        public string InfusionKey { get; set; }
        public string OrderCreatedAtCareUnitID { get; set; }
        public string OrderDoseText { get; set; }
        public string OrderDoseTextSolution { get; set; }
        public List<Preparation> Preparations { get; set; }
        public override string ToString()
        {
            return base.ToString();
            //todo need to write logic for tostring based on template mapping once we have mapping
        }
    }

    public class Preparation
    {
        public string DrugRow { get; set; }
        public string Dose { get; set; }
    }
}
