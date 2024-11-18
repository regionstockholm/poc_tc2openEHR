using TakeCare.Foundation.OpenEhr.Application.Utils;

namespace TakeCare.Migration.OpenEhr.Measurement.Extraction.Model
{
    public class UserDetails
    {
        private string _dateTime;
        public string DateTime
        {
            get
            {
                return _dateTime;
            }
            set
            {
                _dateTime = value.GetFormattedISODate();
            }
        }
        public User User { get; set; }
        public CareUnit CareUnit { get; set; }

    }

    public class CareUnit
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public string FullName { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}