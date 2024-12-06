using TakeCare.Migration.OpenEhr.Application.Utils;
using TakeCare.Migration.OpenEhr.Activities.Extraction.Models;

namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Services
{
    internal class FormatService : IFormatService
    {
        // Format the dates using the ISODateExtension
        public TakeCareActivities Format(TakeCareActivities input)
        {
            foreach (var response in input.Activities)
            {
                response.Guid = Guid.NewGuid(); // Generate a new GUID for each activity
                response.CompletedSetBySystemDateTime = response.CompletedSetBySystemDateTime != null ? response.CompletedSetBySystemDateTime.ToString().GetFormattedISODate() : null;

                if (response.Created != null)
                {
                    response.Created.DateTime = response.Created.DateTime != null ? response.Created.DateTime.ToString().GetFormattedISODate() : null;
                }
                if (response.LastSaved != null)
                {
                    response.LastSaved.DateTime = response.LastSaved.DateTime != null ? response.LastSaved.DateTime.ToString().GetFormattedISODate() : null;
                }
                if (response.Locked != null)
                {
                    response.Locked.DateTime = response.Locked.DateTime != null ? response.Locked.DateTime.ToString().GetFormattedISODate() : null;
                }
                if (response.Signed != null)
                {
                    response.Signed.DateTime = response.Signed.DateTime != null ? response.Signed.DateTime.ToString().GetFormattedISODate() : null;
                }
                if (response.CompletedSetByUserDate != null && response.CompletedSetByUserTime != null)
                {
                    response.CompletedSetByUserDateTime = DataConversion.GetCombinedISODate(response.CompletedSetByUserDate, response.CompletedSetByUserTime);
                }
                if (response.PlannedAt != null)
                {
                    foreach (var plannedDateTime in response.PlannedAt)
                    {
                        plannedDateTime.DateTime = DataConversion.GetCombinedISODate(plannedDateTime.Date, plannedDateTime.Time);
                    }
                }
                if (response.Frequency != null && response.Frequency.Content != null)
                {
                    if (response.Frequency.Content.PlannedDate != null && response.Frequency.Content.PlannedTime != null)
                    {
                        response.Frequency.Content.PlannedDateTime = DataConversion.GetCombinedISODate(response.Frequency.Content.PlannedDate, response.Frequency.Content.PlannedTime);
                    }
                    if (response.Frequency.Content.StartDate != null && response.Frequency.Content.StartTime != null)
                    {
                        response.Frequency.Content.StartDateTime = DataConversion.GetCombinedISODate(response.Frequency.Content.StartDate, response.Frequency.Content.StartTime);
                    }
                    if (response.Frequency.Content.EndDate != null && response.Frequency.Content.EndTime != null)
                    {
                        response.Frequency.Content.EndDateTime = DataConversion.GetCombinedISODate(response.Frequency.Content.EndDate, response.Frequency.Content.EndTime);
                    }
                    if (response.Frequency.Content.Times != null && response.Frequency.Content.Times.Count > 0)
                    {
                        response.Frequency.Content.ConvertedTimes = new List<IntegerTime>();
                        foreach (var time in response.Frequency.Content.Times)
                        {
                            response.Frequency.Content.ConvertedTimes.Add(new IntegerTime
                            {
                                Time = time,
                                FormattedTime = DataConversion.ConvertIntegerToTime(time)
                            });
                        }
                    }
                    if (response.Frequency.Content.IrregularDateTimes != null && response.Frequency.Content.IrregularDateTimes.Count > 0)
                    {
                        for (int i = 0; i < response.Frequency.Content.IrregularDateTimes.Count; i++)
                        {
                            response.Frequency.Content.IrregularDateTimes[i] = response.Frequency.Content.IrregularDateTimes[i].GetFormattedISODate();
                        }
                    }
                }
            }

            return input;
        }
    }
}
