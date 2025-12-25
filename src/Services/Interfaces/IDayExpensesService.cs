using ExpensesCalculator.Models;
using ExpensesCalculator.ViewModels;

namespace ExpensesCalculator.Services
{
    public interface IDayExpensesService
    {
        public string RequestorName { get; set; }

        Task<ICollection<DayExpensesViewModel>> GetAllDays();
        Task<DayExpenses> GetById(int id);
        Task<DayExpensesViewModel> GetDayExpensesViewModelById(int id);
        Task AddDayExpenses(DayExpenses dayExpenses);
        Task EditDayExpenses(DayExpenses dayExpenses);
        Task DeleteDayExpenses(int dayExpensesId);
        Task<DayExpensesCalculationViewModel> GetCalculationForDayExpenses(int id);   
        Task<string?> ChangeDayExpensesAccess(int id, string newUserWithAccess);
    }
}
