namespace TakeCare.Foundation.OpenEhr.Models.Model
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
        public TcDosage()
        {
            DosageDrugs = new List<DosageDrug>(); 
        }
        public override string ToString()
        {
            var dosagemapping = $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/terapeutisk_riktning:0/frekvensschema/schematyp"": {ScheduleType},
                                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/terapeutisk_riktning:0/frekvensschema/period"": ""{Period}"",
                                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/terapeutisk_riktning:0/beställningens_startdatum_tid"": ""{StartFullDateTime}"",
                                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/terapeutisk_riktning:0/dosering:0/dos|unit"": ""1"",";

            return $@"
                    {string.Join("", DosageDrugs.Select(x => x.ToString()))}
                    {dosagemapping}";
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
            return $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/terapeutisk_riktning:0/dosering:0/doseringsbeskrivning"": ""{DoseText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/terapeutisk_riktning:0/dosering:0/dos|magnitude"": ""{DoseNumerical}"",";
        }
    }
}
