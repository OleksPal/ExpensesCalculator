using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;

namespace ExpensesCalculator.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICheckRepository _checkRepository;
        private readonly IDayExpensesRepository _dayExpensesRepository;

        public ItemService(IItemRepository itemRepository, ICheckRepository checkRepository,
            IDayExpensesRepository dayExpensesRepository) 
        {
            _itemRepository = itemRepository;
            _checkRepository = checkRepository;
            _dayExpensesRepository = dayExpensesRepository;
        }        

        public async Task<Item> GetItemById(int id)
        {
            return await _itemRepository.GetById(id);
        }

        public async Task<bool> ItemExists(int id)
        {
            return await GetItemById(id) is not null;
        }

        public async Task<string> GetItemUsers(int id)
        {
            var item = await GetItemById(id);
            string formatList = String.Empty;

            foreach (var user in item.UsersList)
            {
                if (user is not null)
                {
                    formatList += user;
                    if (user != item.UsersList.Last())
                        formatList += ", ";
                }
            }

            return formatList;
        }

        public async Task<MultiSelectList> GetAllAvailableItemUsers(int dayExpensesId)
        {
            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var optionList = new List<SelectListItem>();

            if (dayExpenses is not null)          
            {
                foreach (var participant in dayExpenses.ParticipantsList)
                {
                    optionList.Add(new SelectListItem { Text = participant, Value = participant, Selected = true });
                }
            }

            return new MultiSelectList(optionList, "Value", "Text");
        }

        public async Task<ManageCheckItemsViewModel> AddItem(Item item, int checkId, int dayExpensesId)
        {
            var check = await GetCheckWithItems(checkId);

            check.Items.Add(item);
            check.Sum += item.Price;
            await _itemRepository.Insert(item);
            await _checkRepository.Update(check);            

            return new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
        }

        public async Task<ManageCheckItemsViewModel> EditItem(Item item, int checkId, int dayExpensesId)
        {
            var check = await _checkRepository.GetById(checkId);
            var oldItem = await _itemRepository.GetById(item.Id);
            var oldItemPrice = oldItem.Price;
            
            if (item is not null)
            {
                await _itemRepository.Update(item);
                check.Sum -= oldItemPrice;
                check.Sum += item.Price;                
                check = await _checkRepository.Update(check);
            }

            return new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
        }

        public async Task<ManageCheckItemsViewModel> DeleteItem(int id, int checkId, int dayExpensesId)
        {
            var check = await GetCheckWithItems(checkId);
            var item = await _itemRepository.GetById(id);

            if (item is not null) 
            {
                check.Items.Remove(item);
                check.Sum -= item.Price;
                await _itemRepository.Delete(id);
                await _checkRepository.Update(check);
            }            

            return new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
        }

        private async Task<Check> GetCheckWithItems(int checkId)
        {
            var check = await _checkRepository.GetById(checkId);
            check.Items.AddRange(await _itemRepository.GetAllCheckItems(checkId));

            return check;
        }
    }
}
