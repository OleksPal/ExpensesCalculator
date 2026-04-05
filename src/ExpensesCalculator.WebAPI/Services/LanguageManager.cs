using ExpensesCalculator.Services.Interfaces;
using System.Globalization;

namespace ExpensesCalculator.Services
{
    public class LanguageManager : ILanguageManager
    {
        public readonly List<string> AvailableLanguagesCultures = new List<string>()
        {
            "en-US", "uk-UA"
        };

        public bool IsLanguageCultureAvailable(string language)
        {
            return AvailableLanguagesCultures.Contains(language);
        }

        public void ChangeLanguageCulture(string language)
        {
            try
            {
                if (!IsLanguageCultureAvailable(language))
                    language = "en-US";

                var cultureInfo = new CultureInfo(language);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = cultureInfo;
            }
            catch (Exception) { }
        }
    }
}
