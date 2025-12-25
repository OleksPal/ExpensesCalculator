using ExpensesCalculator.Mappers;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using ExpensesCalculator.Services.Interfaces;
using ExpensesCalculator.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ExpensesCalculator.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IDayExpensesRepository _dayExpensesRepository;

        public ItemService(IItemRepository itemRepository, IDayExpensesRepository dayExpensesRepository) 
        {
            _itemRepository = itemRepository;
            _dayExpensesRepository = dayExpensesRepository;
        }

        public async Task<Item> GetById(int id)
        {
            return await _itemRepository.GetById(id);
        }

        public string GetItemUsers(IEnumerable<string> userList)
        {
            if (userList is not null)
            {
                if (userList.Count() == 1 && userList.ToList()[0].Contains("["))
                    userList = JsonSerializer.Deserialize<List<string>>(userList.ToList()[0]);

                return String.Join(", ", userList);
            }                

            return null;
        }

        public async Task<MultiSelectList> GetAllAvailableItemUsers(int dayExpensesId)
        {
            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var optionList = new List<SelectListItem>();

            if (dayExpenses is not null)          
            {
                foreach (var participant in dayExpenses.Participants)
                    optionList.Add(new SelectListItem { Text = participant, Value = participant });
            }

            return new MultiSelectList(optionList, "Value", "Text");
        }

        public async Task<MultiSelectList> GetCheckedItemUsers(ICollection<string> userList, int dayExpensesId)
        {
            if (userList is null)
                return await GetAllAvailableItemUsers(dayExpensesId);

            if (userList.Count() == 1 && userList.ToList()[0].Contains("["))
                userList = JsonSerializer.Deserialize<List<string>>(userList.ToList()[0]);

            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var optionList = new List<SelectListItem>();

            if (dayExpenses is not null)
            {
                foreach (var participant in dayExpenses.Participants)
                {
                    if (userList.Contains(participant))
                        optionList.Add(new SelectListItem { Text = participant, Value = participant, Selected = true });
                    else
                        optionList.Add(new SelectListItem { Text = participant, Value = participant, Selected = false });
                }                    
            }

            return new MultiSelectList(optionList, "Value", "Text", userList);
        }

        public async Task<Item> AddItemRItem(AddItemViewModel<int> newItemViewModel)
        {
            var itemToAdd = newItemViewModel.ToItem();
            return await _itemRepository.Insert(itemToAdd);
        }

        public async Task AddItem(AddItemViewModel<int> newItemViewModel)
        {
            var itemToAdd = newItemViewModel.ToItem();
            await _itemRepository.Insert(itemToAdd);      
        }

        public async Task EditItem(EditItemViewModel<int> editItemViewModel)
        {
            var editedItem = editItemViewModel.ToItem();
            await _itemRepository.Update(editedItem);
        }

        public async Task DeleteItem(int id)
        {
            await _itemRepository.Delete(id);
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
