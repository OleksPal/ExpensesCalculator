using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface ICheckRepository
    {
        Task<IEnumerable<Check>> GetAll(int dayExpensesId);
        Task<IEnumerable<Check>> GetAllWithItems(int dayExpensesId);
        Task<Check> GetById(int id);
        Task<Check> GetByIdWithItems(int id);
        Task Insert(Check check, int dayExpensesId);
        Task Update(Check check);
        Task Delete(int id);
    }
}
