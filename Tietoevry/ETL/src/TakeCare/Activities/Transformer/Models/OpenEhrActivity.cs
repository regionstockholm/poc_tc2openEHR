using Spine.Migration.OpenEhr.Etl.Core.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class OpenEhrActivity : BaseOpenEhrData
    {

        public TcComposerContext ComposerContext { get; set; }
        public TcCareUnitContext CareUnitContext { get; set; }

        public TcActivityMetadata ContextMetadata { get; set; }

        public TcServiceRequest ServiceRequest { get; set; }

        public OpenEhrActivity()
        {
            
        }

        public override string ToString()
        {
            var result = $@"{ComposerContext.ToString()}    
                            {CareUnitContext.ToString()}
                            {ContextMetadata.ToString()}
                            {ServiceRequest.ToString()}";

            return "{" +
                result.TrimEnd().TrimEnd(',') +

                   "}";
        }
    }
}
