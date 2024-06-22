using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAll(int checkId);
        Task<Item> GetById(int id);
        Task Insert(Item item, int checkId);
        Task Update(Item item, int checkId);
        Task Delete(int id, int checkId);
    }
}
