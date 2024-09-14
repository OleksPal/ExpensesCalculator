using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories.Interfaces
{
    public interface IItemRepository : IGenericRepository<Item>
    {
        Task<ICollection<Item>> GetAllCheckItems(Guid checkId);
        Task<decimal> GetItemPriceById(Guid id);
    }
}
