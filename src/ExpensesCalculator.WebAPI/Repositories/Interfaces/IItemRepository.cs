using ExpensesCalculator.WebAPI.Models;

namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface IItemRepository : IGenericRepository<Item>
{
    Task<ICollection<Item>> GetAllCheckItems(Guid checkId);
}
