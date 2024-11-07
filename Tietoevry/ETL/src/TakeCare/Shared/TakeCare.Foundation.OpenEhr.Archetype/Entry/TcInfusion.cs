namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcInfusion
    {
        public Guid ParentGuid { get; set; }
        public Guid Guid { get; set; }
        public string Row { get; set; }
        public string TimestampSaved { get; set; }
        public string SavedByUserID { get; set; }
        public string SavedAtCareUnitID { get; set; }
        public string EventDatetime { get; set; }
        public string Rate { get; set; }
        public string RateUnit { get; set; }
        public string TotalAmount { get; set; }
        public string Comment { get; set; }
        public string PrescriptionDate { get; set; }
        public string PrescriptionTime { get; set; }
        public string AdministrationKey { get; set; }
        public string OrderCreatedAtCareUnitID { get; set; }

        public override string ToString()
        {
            return base.ToString();
            //todo need to write logic for tostring based on template mapping once we have mapping
        }
    }
}
