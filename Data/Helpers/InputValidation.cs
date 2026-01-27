using System.Text.RegularExpressions;

namespace SecureJournal.Data.Helpers
{
    // Central input validation helper 
    public static class InputValidation
    {
        // Alphabetic characters and spaces only
        private static readonly Regex _alphaWithSpaces =
            new Regex(@"^[A-Za-z\s]*$", RegexOptions.Compiled);

        public static bool IsAlphabeticWithSpaces(string value)
        {
            value ??= "";
            return _alphaWithSpaces.IsMatch(value);
        }

        // Removes numbers and symbols and keeps only letters and spaces
        public static string KeepOnlyLettersAndSpaces(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return Regex.Replace(value, @"[^A-Za-z\s]", "");
        }
    }
}
