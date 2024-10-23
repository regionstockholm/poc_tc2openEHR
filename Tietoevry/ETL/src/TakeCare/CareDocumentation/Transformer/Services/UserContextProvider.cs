using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    internal class UserContextProvider : IUserContextProvider
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
