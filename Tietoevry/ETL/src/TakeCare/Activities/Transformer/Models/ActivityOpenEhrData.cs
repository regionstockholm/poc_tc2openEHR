using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Models
{
    public class ActivityOpenEhrData
    {
        public string PatientId { get; set; }

        public List<OpenEhrActivity> ActivityData { get; set; }

        public ActivityOpenEhrData()
        {
            ActivityData = new List<OpenEhrActivity>();
        }
    }
}
