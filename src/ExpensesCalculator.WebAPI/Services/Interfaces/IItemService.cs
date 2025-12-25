using ExpensesCalculator.WebAPI.Models;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface IItemService
{
    Task<Item> GetById(Guid id);
    Task AddItem(Item item);
    Task EditItem(Item item);
    Task DeleteItem(Guid id);
}
