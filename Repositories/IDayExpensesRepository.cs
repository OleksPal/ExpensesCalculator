using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IDayExpensesRepository
    {
        Task<IEnumerable<DayExpenses>> GetAll();
        Task<DayExpenses> GetById(int id);
        Task Insert(DayExpenses dayExpenses);
        Task Update(DayExpenses dayExpenses);
        Task Delete(int id);
    }
}
