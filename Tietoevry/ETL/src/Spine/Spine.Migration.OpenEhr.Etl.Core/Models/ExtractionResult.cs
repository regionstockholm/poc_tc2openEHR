namespace Spine.Migration.OpenEhr.Etl.Core.Models
{
    public class ExtractionResult<TResult>
    {
        public ExtractionResult(TResult result)
        {
            Result = result;
        }

        public TResult Result { get; }
    }
}
