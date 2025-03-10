using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models
{
    public class ChemistryReply
    {
        private string? comment;

        public string? Comment
        {
            get => comment;
            set => comment = value?.Replace("\n", "\\n").Replace("\r", "\\n");
        }

        //public string Comment { get; set; }
        public string Id { get; set; }
        public string InvoiceeCareUnitExternalId { get; set; }
        public string OrdererCareUnitExternalId { get; set; }

        private string? orderComment;

        public string? OrderComment
        {
            get => orderComment;
            set => orderComment = value?.Replace("\n", "\\n").Replace("\r", "\\n");
        }

        //public string OrderComment { get; set; }
        public string OriginalRecipientsAddress { get; set; }
        public string ReferralId { get; set; }
        public string ReferringDoctor { get; set; }
        public string? ReplyTime { get; set; }
        public List<Sample>? Samples { get; set; }
        public string? SamplingDateTime { get; set; }
        public string VersionId { get; set; }
        public string LID { get; set; }
        public Attestation Attestation { get; set; }
        public bool IsCopy { get; set; }
        public bool? IsFinal { get; set; }
        public Lab Lab { get; set; }
        public Order? Order { get; set; }
        public Saved? Saved { get; set; }
        public Type Type { get; set; }
    }
}
