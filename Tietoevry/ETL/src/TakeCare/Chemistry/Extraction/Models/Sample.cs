using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class Sample
    {
        public List<Analysis> Analyses { get; set; }
        public string Group { get; set; }

        private string? groupComment;

        public string? GroupComment
        {
            get => groupComment;
            set => groupComment = value?.Replace("\n", "\\n").Replace("\r", "\\n");
        }

        //public string GroupComment { get; set; }
    }
}
