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
        Task<Item> SetCheck(Item item);
        Task<Check> AddItem(AddItemViewModel<int> item);
        Task<Check> EditItem(EditItemViewModel<int> item);
        Task<Check> DeleteItem(int id);
    }
}
