using System;
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
    }

}
