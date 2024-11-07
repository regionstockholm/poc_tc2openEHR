using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.Measurement.Transformer.Model
{
    public class OpenEhrMeasurement
    {
        public string PatientID { get; set; }
        public Context Context { get; set; }
        public TcCareUnitContext CareUnitContext { get; set; }
        public TcContextMetadata ContextMetadata { get; set; }
        public TcContextInformation ContextInformation { get; set; }

        public List<BaseEntry> Measurements = new List<BaseEntry>();

        override public string ToString()
        {
            var result = $@"{Context.ToString()}
                            {CareUnitContext.ToString()}
                            {ContextMetadata.ToString()}
                            {ContextInformation.ToString()}";

            foreach (var measurement in Measurements)
            {
                result += $@"{measurement.ToString()}";
            }

            return "{"+result.TrimEnd().TrimEnd(',')+"}";
        }
    }
}
