using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeCare.Migration.OpenEhr.Application.Models
{
    public class CompositionConstants
    {
        /// <summary>
        /// Schema ID for Healthcare Facility
        /// </summary>
        public const string SCHEMA_ID = "HOSPITAL-NS";
        
        /// <summary>
        /// Namespace ID for Healthcare Facility
        /// </summary>
        public const string NAMESPACE_ID = "HOSPITAL-NS";
        
        /// <summary>
        /// OID marker for swedish care unit HSA-IDs
        /// </summary>
        public const string CARE_UNIT_HSA_ID_OID_MARKER = "urn:oid:1.2.752.29.4.19";

        public const string CARE_PROVIDER_TYPE = "urn:oid:2.5.4.97";

    }
}
