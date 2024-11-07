namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
{
    public class TcDosage
    {
        public Guid ParentGuid { get; set; }
        public Guid Guid { get; set; }                                                                                      
        public string DosageID { get; set; }
        public string TimestampSaved { get; set; }
        public string SavedByUserID { get; set; }
        public string SavedAtCareUnitID { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string StartFullDateTime { get; set; }
        public string ScheduleType { get; set; }
        public string Period { get; set; }
        public string IsGivenOnMondays { get; set; }
        public string IsGivenOnTuesdays { get; set; }
        public string IsGivenOnWednesdays { get; set; }
        public string IsGivenOnThursdays { get; set; }
        public string IsGivenOnFridays { get; set; }
        public string IsGivenOnSaturdays { get; set; }
        public string IsGivenOnSundays { get; set; }
        public List<DosageDrug> DosageDrugs { get; set; }
        public override string ToString()
        {
            var resultString = string.Empty;
            DosageDrugs.ForEach(x => { string.Concat(resultString, x.ToString()); });

            return $@"
                    {resultString}
                    {base.ToString()}";
            //todo need to write logic for tostring based on template mapping once we have mapping
        }
    }

    public class DosageDrug
    {
        public string DrugRow { get; set; }
        public string DrugCode { get; set; }
        public string DoseText { get; set; }
        public string DoseNumerical { get; set; }
        public override string ToString()
        {
            return base.ToString();
            //todo need to write logic for tostring based on template mapping once we have mapping
        }
    }
}
