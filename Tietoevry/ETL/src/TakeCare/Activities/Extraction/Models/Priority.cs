using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
