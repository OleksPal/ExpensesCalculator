using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllCheckItems(int checkId);
        Task<Item> GetById(int id);
        Task<Item> Insert(Item item);
        Task<Item> Update(Item item);
        Task<Item> Delete(int id);
    }
}
