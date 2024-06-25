using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IDayExpensesRepository
    {
        Task<IEnumerable<DayExpenses>> GetAll();
        Task<DayExpenses> GetById(int id);
        Task<DayExpenses> Insert(DayExpenses dayExpenses);
        Task<DayExpenses> Update(DayExpenses dayExpenses);
        Task<DayExpenses> Delete(int id);
    }
}
