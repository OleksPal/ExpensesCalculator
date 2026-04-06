namespace ExpensesCalculator.Services.Interfaces
{
    public interface ILanguageManager
    {
        bool IsLanguageCultureAvailable(string language);
        void ChangeLanguageCulture(string language);
    }
}
