namespace Spine.Migration.OpenEhr.Etl.Core
{
    public interface ILoader
    {
        Task<TResult> Load<TInput, TResult>(TInput input) where TInput : class where TResult : class;
    }
}
