using Spine.Migration.OpenEhr.Etl.Core.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class ChemistryOpenEhrData
    {
        public string PatientID { get; set; }
        public List<OpenEhrChemistry> ChemistryData { get; set; }
        public ChemistryOpenEhrData()
        {
            ChemistryData = new List<OpenEhrChemistry>();
        }
    }
}
