namespace TakeCare.Migration.OpenEhr.Application.Models
{
    public class ContextDetails
    {
        /// <summary>
        /// Take Care Unit ID
        /// </summary>
        public string TakeCareUnitId { get; set; }

        /// <summary>
        /// Care Unit ID
        /// </summary>
        public string CareUnitId { get; set; }

        /// <summary>
        /// Care Unit Name
        /// </summary>
        public string CareUnitName { get; set; }

        /// <summary>
        /// Care Provider ID
        /// </summary>
        public string CareProviderId { get; set; }

        /// <summary>
        /// Care Provider Name
        /// </summary>
        public string CareProviderName { get; set; }

        /// <summary>
        /// Healthcare Facility ID
        /// </summary>
        public string HealthCareFacilityId { get; set; }

        /// <summary>
        /// Healthcare Facility Name
        /// </summary>
        public string HealthCareFacilityName { get; set; }

        public string OrganisationNumber { get; set; }

    }
}
