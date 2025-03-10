namespace TakeCare.Migration.OpenEhr.Application.Services
{
    public interface IUserContextProvider
    {
        public string GetUserContextData(string userId);
    }
}
