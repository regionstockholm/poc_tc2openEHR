﻿namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class TcDay
    {
        private const string _prefix = "läkemedelsförskrivning/läkemedelsbeställning";
        public Guid ParentGuid { get; set; }
        public Guid Guid { get; set; }
        public string Row { get; set; }
        public string TimestampSaved { get; set; }
        public string SavedByUserID { get; set; }
        public string SavedAtCareUnitID { get; set; }
        public string SignerUserID { get; set; }
        public string AdministrationFullStartDateTime { get; set; }
        public string MaxDailyDose { get; set; }
        public string InfusionTime { get; set; }
        public string DosageInstruction { get; set; }
        public string DosageInstructionTemplate { get; set; }
        public string IsSelfAdministered { get; set; }
        public override string ToString()
        {
            var infusionTime = string.IsNullOrWhiteSpace(InfusionTime) ? string.Empty : $@"PT{InfusionTime}M";
            var result =
                 $@"""{_prefix}/order:0/ytterligare_instruktioner"": ""{DosageInstruction}"",
                    ""{_prefix}/order:0/terapeutisk_riktning:0/dosering:0/administrationens_varaktighet"": ""{infusionTime}"",";

            return result;
        }
    }
}
