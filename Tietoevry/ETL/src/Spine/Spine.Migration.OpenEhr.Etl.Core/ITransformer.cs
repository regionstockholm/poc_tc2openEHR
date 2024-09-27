using Spine.Migration.OpenEhr.Etl.Core.Models;

namespace Spine.Migration.OpenEhr.Etl.Core
{
    public interface ITransformer
    {
        Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input) where TInput : class where TResult : class;
    }
}
