using Microsoft.Extensions.Logging;
using Spine.Migration.OpenEhr.Etl.Core;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using Spine.Migration.OpenEhr.Loader;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.Etl.CareDocumentation;

namespace TakeCare.Migration.OpenEhr.Etl.Handlers
{
    public class CareDocumentationEtlHandler //: IEtlHandler
    {
        private readonly Lazy<ICareDocumentationExtractor> _lazyExtractor;
        private readonly Lazy<ICareDocumentationTransformer> _lazyTransformer;
        private readonly Lazy<IOpenEhrLoader> _lazyLoader;
        private readonly Lazy<ILogger<CareDocumentationEtlHandler>> _lazyLogger;
        private ILogger<CareDocumentationEtlHandler> _logger => _lazyLogger.Value;
        private ICareDocumentationExtractor _careDocExtractor => _lazyExtractor.Value;
        private ICareDocumentationTransformer _careDocTransformer => _lazyTransformer.Value;
        private IOpenEhrLoader _careDocOpenEhrLoader => _lazyLoader.Value;

        public CareDocumentationEtlHandler(Lazy<ICareDocumentationExtractor> lazyExtractor,
            Lazy<ICareDocumentationTransformer> lazyTransformer, Lazy<IOpenEhrLoader> lazyLoader,
            Lazy<ILogger<CareDocumentationEtlHandler>> lazyLogger)
        {
            _lazyExtractor = lazyExtractor;
            _lazyTransformer = lazyTransformer;
            _lazyLoader = lazyLoader;
            _lazyLogger = lazyLogger;
        }


        public async Task Execute()
        {
            // ToDo 
            // Looping for Parllel CareDocumentation ETL execution
            int count = 0;
            string careDocsFolder = Path.Combine(AppContext.BaseDirectory, @"Assets\TestData\CareDocumentation");
            foreach (var file in Directory.EnumerateFiles(careDocsFolder, "*.xml"))
            {
                try
                {
                    //string file = Path.Combine(AppContext.BaseDirectory, @"TestData\CareDocumentationGet_201402192387_0 - Mock.xml");
                    var extractorConfigurations = new ExtractionConfiguration<string>(file);
                    var tcData = await _careDocExtractor.Extract<string, CareDocumentationDto>(extractorConfigurations);
                    var tcOpenEhrData = await _careDocTransformer.Trasform<CareDocumentationDto, CareDocumentOpenEhrData>(tcData);
                    var result = await _careDocOpenEhrLoader.Load<OpenEhrData<OpenEhrCaseNote>, object>(new OpenEhrData<OpenEhrCaseNote>()
                    {
                        PatientID = tcOpenEhrData.PatientID,
                        Compositions = tcOpenEhrData.CaseNotes
                    });  // ToDo Mapper and model optimization
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in processing file {file}");
                }

                _logger.LogInformation($"{++count}. File name : {file}");
            }
        }

    }
}
