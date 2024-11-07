using System.Globalization;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction.Extension
{
    public static class ISODateExtension
    {
        public static string GetFormattedISODate(this String inputText)
        {
            inputText = inputText.Length != 14 ? inputText.PadRight(14, '0') : inputText;

            DateTime date = DateTime.ParseExact(inputText, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return date.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}
