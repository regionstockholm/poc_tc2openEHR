using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Services
{
    public interface ITransformService
    {
        ChemistryOpenEhrData Transform(TakeCareChemistry inputData);
    }
}
