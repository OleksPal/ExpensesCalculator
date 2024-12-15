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
    }
}
