using ExpensesCalculator.Models;
using ExpensesCalculator.ViewModels;

namespace ExpensesCalculator.Mappers
{
    public static class ItemMappers
    {
        public static Item ToItem(this AddItemViewModel<int> addItemViewModel)
        {
            return new Item
            {
                Name = addItemViewModel.Name,
                Description = addItemViewModel.Description,
                Price = addItemViewModel.Price * addItemViewModel.Amount,
                UsersList = addItemViewModel.UserList,
                CheckId = addItemViewModel.CheckId
            };
        }

        public static Item ToItem(this EditItemViewModel<int> editItemViewModel)
        {
            return new Item
            {
                Id = editItemViewModel.Id,
                Name = editItemViewModel.Name,
                Description = editItemViewModel.Description,
                Price = editItemViewModel.Price * editItemViewModel.Amount,
                UsersList = editItemViewModel.UserList,
                CheckId = editItemViewModel.CheckId
            };
        }

        public static EditItemViewModel<int> ToEditItemViewModel(this Item item)
        {
            return new EditItemViewModel<int>
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Amount = 1,
                UserList = item.UsersList.ToList(),
                CheckId = item.CheckId
            };
        }

        public static AddItemViewModel<int> ToAddItemViewModel(this Item item)
        {
            return new AddItemViewModel<int>
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Amount = 1,
                UserList = item.UsersList.ToList(),
                CheckId = item.CheckId
            };
        }
    }
}
