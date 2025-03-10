using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class Signed
    {
        public string? DateTime { get; set; }
        public UserData User { get; set; }
    }
}
