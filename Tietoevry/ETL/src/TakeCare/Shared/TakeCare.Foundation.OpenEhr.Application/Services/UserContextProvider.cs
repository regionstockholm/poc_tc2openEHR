using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Foundation.OpenEhr.Application.Services
{
    public class UserContextProvider : IUserContextProvider
    {
        private static List<UserContextDetails> _users { get; set; }
        static UserContextProvider()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "UserContext.json");
            _users = Utility.LoadData<List<UserContextDetails>>(filePath);
        }
        public string GetUserContextData(string userId)
        {
            UserContextDetails userdata = _users.Find(t => t.TakeCareUserID.Equals(userId));
            return userdata!=null? userdata.TakeCareUserName :  "";
        }
    }
}
