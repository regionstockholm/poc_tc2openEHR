using System.Globalization;

namespace TakeCare.Foundation.OpenEhr.Application.Utils
{
    public static class ISODateExtension
    {
        public static string GetFormattedISODate(this String inputText)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(inputText))
                {
                    var formattedDate = inputText;
                    formattedDate = formattedDate.Length != 14 ? formattedDate.PadRight(14, '0') : formattedDate;

                    DateTime date = DateTime.ParseExact(formattedDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    return date.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                return inputText;
            }
            catch (System.FormatException ex)
            {
                var exMessage = "Invalid date - " + inputText;
                throw new System.FormatException(exMessage, ex);
            }
        }
    }
}