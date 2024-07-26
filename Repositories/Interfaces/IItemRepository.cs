using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories.Interfaces
{
    public interface IItemRepository : IGenericRepository<Item>
    {
        Task<ICollection<Item>> GetAllCheckItems(int checkId);
        Task<decimal> GetItemPriceById(int id);
    }
}
