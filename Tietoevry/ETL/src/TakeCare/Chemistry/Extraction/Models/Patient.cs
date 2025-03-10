using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class Patient
    {
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Id { get; set; }
        public string IdFormatted { get; set; }
        public string LastName { get; set; }
    }
}
