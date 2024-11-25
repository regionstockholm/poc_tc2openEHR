using System.Globalization;

namespace TakeCare.Foundation.OpenEhr.Application.Utils
{
    public static class DataConversion
    {
        public static string GetNumberValue(this string number)
        {
            var swedishCulture = new CultureInfo("sv-SE");

            // Try to parse the number as a decimal
            if (decimal.TryParse(number, NumberStyles.Any, swedishCulture, out decimal decimalResult))
            {
                // Convert the decimal to a string using invariant culture
                return decimalResult.ToString(CultureInfo.InvariantCulture);
            }

            // If parsing fails, return the original string
            return number.Replace("\n", "\\n").Replace("\r", "\\n");
        }

        public static decimal? GetDecimalNumberValue(this string number)
        {
            var swedishCulture = new CultureInfo("sv-SE");

            // Try to parse the number as a decimal
            if (decimal.TryParse(number, NumberStyles.Any, swedishCulture, out decimal decimalResult))
            {
                // Convert the decimal to a string using invariant culture
                return decimalResult;
            }

            // If parsing fails, return the original string
            return null;
        }


        public static string? ConvertIntegerToTime(int input)
        {
            if (input < 0 || input > 2359)
                throw new ArgumentOutOfRangeException("Input must be between 0 and 2359 in Frequency Content");

            // Extract hours and minutes
            int hours = input / 100;
            int minutes = input % 100;

            // Ensure minutes are valid
            if (minutes < 0 || minutes >= 60)
                throw new ArgumentOutOfRangeException("Invalid minutes in Frequency Content");

            // Format the time as HH:mm
            return string.Format("{0:D2}:{1:D2}", hours, minutes);
        }

        public static string GetCombinedISODate(int? givenDate, int? givenTime)
        {
            // Convert plannedDate and plannedTime to strings and pad with zeros if necessary
            string dateText = givenDate.ToString().PadLeft(8, '0');
            string timeText = givenTime.ToString().PadLeft(4, '0');

            // Combine the date and time strings
            string dateTimeText = dateText + timeText;

            // Parse the combined string into a DateTime object
            DateTime date;
            if (!DateTime.TryParseExact(dateTimeText, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                throw new FormatException("Input text is not in the correct format 'yyyyMMddHHmm'.");
            }

            // Return the formatted ISO 8601 date-time string
            return date.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }

}
