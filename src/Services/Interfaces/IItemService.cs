using ExpensesCalculator.Models;
using ExpensesCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public interface IItemService
    {        
        Task<Item> GetItemById(int id);
        Task<string> GetItemUsers(IEnumerable<string> userList);
        Task<MultiSelectList> GetAllAvailableItemUsers(int dayExpensesId);
        Task<MultiSelectList> GetCheckedItemUsers(ICollection<string> userList, int dayExpensesId);
        Task<Item> AddItem(AddItemViewModel<int> item);
        Task<Item> EditItem(EditItemViewModel<int> item);
        Task<Item> DeleteItem(int id);
    }
}
