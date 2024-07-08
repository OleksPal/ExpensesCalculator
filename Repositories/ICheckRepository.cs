using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface ICheckRepository
    {
        Task<IEnumerable<Check>> GetAllDayChecks(int dayExpensesId);
        Task<Check> GetById(int id);
        Task<Check> Insert(Check check);
        Task<Check> Update(Check check);
        Task<Check> Delete(int id);
    }
}
