using ExpensesCalculator.Models;
using ExpensesCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services.Interfaces;

public interface IItemService
{
    Task<Item> GetById(int id);
    string GetItemUsers(IEnumerable<string> userList);
    Task<MultiSelectList> GetAllAvailableItemUsers(int dayExpensesId);
    Task<MultiSelectList> GetCheckedItemUsers(ICollection<string> userList, int dayExpensesId);
    Task AddItem(AddItemViewModel<int> item);
    Task EditItem(EditItemViewModel<int> item);
    Task DeleteItem(int id);
}
