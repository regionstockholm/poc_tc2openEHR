using Spine.Migration.OpenEhr.Etl.Core;

namespace TakeCare.Migration.OpenEhr.Etl
{
    using Handlers;

    public class TakeCareEtlHandler : IEtlHandler
    {
        ICareDocumentationEtlHandler _careDocumentationEtlHandler;

        public TakeCareEtlHandler(ICareDocumentationEtlHandler careDocumentationEtlHandler)
        {
            _careDocumentationEtlHandler = careDocumentationEtlHandler;
        }

        public async void Execute()
        {
            // ToDo
            // TakeCare CareDocumentation ETL
            // TakeCare Measurements ETL
            // TakeCare MedicationHistory ETL
            // TakeCare Chemistry ETL
            //  ...n

            // Looping for (TPL) Parllel all TakeCare ETL execution
            _careDocumentationEtlHandler.Execute();
        }
    }
}
