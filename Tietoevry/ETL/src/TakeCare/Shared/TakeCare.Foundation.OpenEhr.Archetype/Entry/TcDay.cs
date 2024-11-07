namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcDay
    {
        public Guid ParentGuid { get; set; }
        public Guid Guid { get; set; }
        public string Row { get; set; }
        public string TimestampSaved { get; set; }
        public string SavedByUserID { get; set; }
        public string SavedAtCareUnitID { get; set; }
        public string SignerUserID { get; set; }
        public string AdministrationStartDate { get; set; }
        public string AdministrationStartTime { get; set; }
        public string MaxDailyDose { get; set; }
        public string InfusionTime { get; set; }
        public string DosageInstruction { get; set; }
        public string DosageInstructionTemplate { get; set; }
        public override string ToString()
        {
            return base.ToString();
            //todo need to write logic for tostring based on template mapping once we have mapping
        }
    }
}
