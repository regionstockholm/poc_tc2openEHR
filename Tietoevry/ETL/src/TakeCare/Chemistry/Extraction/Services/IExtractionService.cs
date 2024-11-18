using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Services
{
    public interface IExtractionService
    {
        TakeCareChemistry ExtractChemistryData(string file);
    }
}
