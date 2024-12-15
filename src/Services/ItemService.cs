using ExpensesCalculator.Mappers;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using ExpensesCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

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

        public async Task<Item> SetCheck(Item item)
        {
            if (item.CheckId != 0)
                item.Check = await _checkRepository.GetById(item.CheckId);

            return item;
        }

        public async Task<Item> GetItemById(int id)
        {
            return await _itemRepository.GetById(id);
        }

        public async Task<string> GetItemUsers(IEnumerable<string> userList)
        {
            if (userList is not null)
                return String.Join(", ", userList);

            return null;
        }

        public async Task<MultiSelectList> GetAllAvailableItemUsers(int dayExpensesId)
        {
            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var optionList = new List<SelectListItem>();

            if (dayExpenses is not null)          
            {
                foreach (var participant in dayExpenses.ParticipantsList)
                    optionList.Add(new SelectListItem { Text = participant, Value = participant });
            }

            return new MultiSelectList(optionList, "Value", "Text");
        }

        public async Task<MultiSelectList> GetCheckedItemUsers(ICollection<string> userList, int dayExpensesId)
        {
            if (userList is null)
                return await GetAllAvailableItemUsers(dayExpensesId);

            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var optionList = new List<SelectListItem>();

            if (dayExpenses is not null)
            {
                foreach (var participant in dayExpenses.ParticipantsList)
                {
                    if (userList.Contains(participant))
                        optionList.Add(new SelectListItem { Text = participant, Value = participant, Selected = true });
                    else
                        optionList.Add(new SelectListItem { Text = participant, Value = participant, Selected = false });
                }                    
            }

            return new MultiSelectList(optionList, "Value", "Text", userList);
        }

        public async Task<Check> AddItem(AddItemViewModel<int> newItemViewModel)
        {
            var check = await GetCheckWithItems(newItemViewModel.CheckId);

            var itemToAdd = newItemViewModel.ToItem();

            check.Sum += itemToAdd.Price;
            await _itemRepository.Insert(itemToAdd);
            check = await _checkRepository.Update(check);            

            return check;
        }

        public async Task<Check> EditItem(Item item)
        {
            string rareNameList = item.UsersList.First();
            item.UsersList = GetUserListFromString(rareNameList);

            var check = await _checkRepository.GetById(item.CheckId);
            var oldItemPrice = await _itemRepository.GetItemPriceById(item.Id);

            if (item is not null)
            {
                await _itemRepository.Update(item);
                check.Sum -= oldItemPrice;
                check.Sum += item.Price;                
                await _checkRepository.Update(check);
                check = await GetCheckWithItems(item.CheckId);
            }

            return check;
        }

        public async Task<Check> DeleteItem(int id)
        {
            var item = await _itemRepository.GetById(id);
            var check = await GetCheckWithItems(item.CheckId);            

            if (item is not null) 
            {                
                check.Sum -= item.Price;
                await _itemRepository.Delete(id);
                await _checkRepository.Update(check);
                check = await GetCheckWithItems(item.CheckId);
            }            

            return check;
        }

        private async Task<Check> GetCheckWithItems(int checkId)
        {
            var check = await _checkRepository.GetById(checkId);
            check.Items = await _itemRepository.GetAllCheckItems(checkId);

            return check;
        }

        private ICollection<string> GetUserListFromString(string rareText)
        {
            Regex pattern = new Regex(@"\w+");

            var matchList = pattern.Matches(rareText).ToList();
            List<string> participantList = new List<string>();

            foreach (var match in matchList)
                participantList.Add(match.Value);

            return participantList;
        }
    }
}
