using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public interface IItemService
    {        
        Task<Item> GetItemById(Guid id);
        Task<string> GetItemUsers(IEnumerable<string> userList);
        Task<MultiSelectList> GetAllAvailableItemUsers(Guid dayExpensesId);
        Task<MultiSelectList> GetCheckedItemUsers(Item item, Guid dayExpensesId);
        Task<Item> SetCheck(Item item);
        Task<Check> AddItem(Item item);
        Task<Check> EditItem(Item item);
        Task<Check> DeleteItem(Guid id);
    }
}
