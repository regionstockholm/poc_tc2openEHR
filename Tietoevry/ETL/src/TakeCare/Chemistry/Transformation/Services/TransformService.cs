using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Services
{
    internal class TransformService : ITransformService
    {
        private readonly ICompositionService _compositionService;

        public TransformService(ICompositionService compositionService)
        {
            _compositionService = compositionService;
        }
        public ChemistryOpenEhrData Transform(TakeCareChemistry inputData)
        {
            return _compositionService.Compose(inputData);
        }
    }
}
