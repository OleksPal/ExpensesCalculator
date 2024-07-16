using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public interface IItemService
    {        
        Task<Item> GetItemById(int id);
        Task<bool> ItemExists(int id);
        Task<string> GetItemUsers(int id);
        Task<MultiSelectList> GetAllAvailableItemUsers(int dayExpensesId);
        Task<Check> AddItem(Item item, int checkId, int dayExpensesId);
        Task<Check> EditItem(Item item, int checkId, int dayExpensesId);
        Task<Check> DeleteItem(int id, int checkId, int dayExpensesId);
    }
}
