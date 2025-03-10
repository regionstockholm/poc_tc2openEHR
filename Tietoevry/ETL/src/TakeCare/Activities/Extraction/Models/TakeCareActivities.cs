using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class TakeCareActivities
    {
        public string PatientId { get; set; }
        public List<Activity> Activities { get; set; }

        public TakeCareActivities()
        {
            Activities = new List<Activity>();
        }
    }
}
