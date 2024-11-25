using TakeCare.Foundation.OpenEhr.Application.Utils;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Extraction.Services
{
    internal class FormatService : IFormatService
    {
        // Format the dates using the ISODateExtension
        public TakeCareChemistry Format(TakeCareChemistry input)
        {
            foreach (var response in input.ChemistryData)
            {
                response.ReplyTime = response.ReplyTime != null ? response.ReplyTime.ToString().GetFormattedISODate() : null;
                response.SamplingDateTime = response.SamplingDateTime != null ? response.SamplingDateTime.ToString().GetFormattedISODate() : null;

                if (response.Attestation != null)
                {
                    response.Attestation.CreatedDateTime = response.Attestation.CreatedDateTime != null ? response.Attestation.CreatedDateTime.ToString().GetFormattedISODate() : null;
                    if (response.Attestation.Document != null && response.Attestation.Document.SavedDateTime != null)
                    {
                        response.Attestation.Document.SavedDateTime = response.Attestation.Document.SavedDateTime.ToString().GetFormattedISODate();
                    }
                    if (response.Attestation.Attested != null && response.Attestation.Attested.DateTime != null)
                    {
                        response.Attestation.Attested.DateTime = response.Attestation.Attested.DateTime.ToString().GetFormattedISODate();
                    }
                }

                if (response.Saved != null && response.Saved.SavedTimestamp != null)
                {
                    response.Saved.SavedTimestamp = response.Saved.SavedTimestamp.ToString().GetFormattedISODate();
                }
            }

            return input;
        }
    }
}
