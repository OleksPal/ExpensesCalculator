using ExpensesCalculator.Models;

namespace ExpensesCalculator.Services
{
    public interface IDayExpensesService
    {
        public string RequestorName { get; set; }

        Task<ICollection<DayExpenses>> GetAllDays();
        Task<DayExpenses> GetDayExpensesById(Guid id);
        Task<DayExpenses> GetDayExpensesByIdWithChecks(Guid id);
        Task<DayExpenses> GetFullDayExpensesById(Guid id);
        Task<DayExpenses> AddDayExpenses(DayExpenses dayExpenses);
        Task<DayExpenses> EditDayExpenses(DayExpenses dayExpenses);
        Task<DayExpenses> DeleteDayExpenses(Guid dayExpensesId);
        Task<DayExpensesCalculationViewModel> GetCalculationForDayExpenses(Guid id);
        Task<string> GetFormatParticipantsNames(IEnumerable<string> participantList);        
        Task<string?> ChangeDayExpensesAccess(Guid id, string newUserWithAccess);
    }
}
