using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public interface IItemService
    {        
        Task<Item> GetItemById(int id);
        Task<string> GetItemUsers(int id);
        Task<MultiSelectList> GetAllAvailableItemUsers(int dayExpensesId);
        Task<MultiSelectList> GetCheckedItemUsers(Item item, int dayExpensesId);
        Task<Item> SetCheck(Item item);
        Task<Check> AddItem(Item item);
        Task<Check> EditItem(Item item);
        Task<Check> DeleteItem(int id);
    }
}
