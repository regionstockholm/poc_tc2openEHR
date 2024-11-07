using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class OpenEhrMedication
    {
        public string PatientID { get; set; }
        public Context Context { get; set; }
        public TcContextInformation ContextInformation { get; set; }
        public TcCareUnitContext CareUnitContext { get; set; }
        public TcMedicationContext MedicationContext { get; set; }
        public TcMedicationOrder Medication { get; set; }
        public TcPrescription Prescription { get; set; }
        public List<TcAdministration> Administrations { get; set; }
        public List<TcDrug> Drugs { get; set; }
        public List<TcDosage> Dosages { get; set; }
        public List<TcDay> Days { get; set; }
        public List<TcInfusion> Infusions { get; set; }
        public OpenEhrMedication() {
            Administrations = new List<TcAdministration>();
            Infusions = new List<TcInfusion>();
            Days = new List<TcDay>();
            Drugs = new List<TcDrug>();
            Dosages = new List<TcDosage>();
        }
        override public string ToString()
        {
            //var drugs = Drugs.Count > 0 ? string.Join("", Drugs.Select(x => x.ToString())) : string.Empty;
            //var dosages = Dosages.Count > 0 ? string.Join("", Drugs.Select(x => x.ToString())) : string.Empty;
            //var days = Days.Count > 0 ?  string.Join("", Drugs.Select(x => x.ToString())) : string.Empty;
            //var admins = Administrations.Count > 0 ? string.Join("", Drugs.Select(x => x.ToString())) : string.Empty;
            //var infusions = Infusions.Count > 0 ? string.Join("", Drugs.Select(x => x.ToString())) : string.Empty;

            var result = $@"{Context.ToString()}
                            {ContextInformation.ToString()}
                            {MedicationContext.ToString()}
                            {Medication.ToString()}
                            {Prescription.ToString()}
                            {string.Join("", Drugs.Select(x => x.ToString()))}
                            {string.Join("", Dosages.Select(x => x.ToString()))}
                            {string.Join("", Days.Select(x => x.ToString()))}";
                            //{string.Join("", Administrations.Select(x => x.ToString()))}
                            //{string.Join("", Infusions.Select(x => x.ToString()))}";

            return "{" + result.TrimEnd().TrimEnd(',') + "}";
        }
    }
}
