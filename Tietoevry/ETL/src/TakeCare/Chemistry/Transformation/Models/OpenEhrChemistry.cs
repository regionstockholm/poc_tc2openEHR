using Spine.Migration.OpenEhr.Etl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models
{
    public class OpenEhrChemistry : BaseOpenEhrData
    {
        public TcContextReportID ReportID { get; set; }
        public TcChemistryCareUnitContext CareUnitContext { get; set; }
        public TcChemistryContextMetadata ContextMetadata { get; set; }
        public TcChemistryAttestationData AttestationData { get; set; }

        public List<TcLabResult> TestResult { get; set; }

        public OpenEhrChemistry()
        {
            TestResult = new List<TcLabResult>();
        }
        public override string ToString()
        {
            var result = $@"
                            {ReportID.ToString()}
                            {CareUnitContext.ToString()}
                            {ContextMetadata.ToString()}
                            {AttestationData.ToString()}";
            
            foreach(var labResult in TestResult)
            {
                result += labResult.ToString();
            }
            return "{" + 
                        result.TrimEnd().TrimEnd(',') + 
                   "}";
        }
    }
}
