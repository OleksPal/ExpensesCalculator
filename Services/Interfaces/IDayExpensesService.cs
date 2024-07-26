using ExpensesCalculator.Models;

namespace ExpensesCalculator.Services
{
    public interface IDayExpensesService
    {
        public string RequestorName { get; set; }

        Task<ICollection<DayExpenses>> GetAllDays();
        Task<DayExpenses> GetDayExpensesById(int id);
        Task<DayExpenses> GetDayExpensesByIdWithChecks(int id);
        Task<DayExpenses> GetFullDayExpensesById(int id);
        Task<DayExpenses> AddDayExpenses(DayExpenses dayExpenses);
        Task<DayExpenses> EditDayExpenses(DayExpenses dayExpenses);
        Task<DayExpenses> DeleteDayExpenses(int dayExpensesId);
        Task<DayExpensesCalculationViewModel> GetCalculationForDayExpenses(int id);
        Task<string> GetFormatParticipantsNames(IEnumerable<string> participantList);        
        Task<string?> ChangeDayExpensesAccess(int id, string newUserWithAccess);
    }
}
