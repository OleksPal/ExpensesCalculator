using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public class CheckService : ICheckService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICheckRepository _checkRepository;
        private readonly IDayExpensesRepository _dayExpensesRepository;
        
        public CheckService(IItemRepository itemRepository, ICheckRepository checkRepository,
            IDayExpensesRepository dayExpensesRepository)
        {
            _itemRepository = itemRepository;
            _checkRepository = checkRepository;
            _dayExpensesRepository = dayExpensesRepository;
        }

        public async Task<Check> SetDayExpenses(Check check)
        {
            if (check.DayExpensesId != 0) 
                check.DayExpenses = await _dayExpensesRepository.GetById(check.DayExpensesId);

            return check;
        }

        public async Task<Check> GetCheckById(int id)
        {
            return await _checkRepository.GetById(id);
        }

        public async Task<Check> GetCheckByIdWithItems(int id)
        {
            var check = await GetCheckById(id);
            var items = await _itemRepository.GetAllCheckItems(id);

            if (items.Count > 0)
                check.Items = items;

            return check;
        }

        public async Task<SelectList> GetAllAvailableCheckPayers(int dayExpensesId)
        {
            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var optionList = new List<SelectListItem>();

            if (dayExpenses is not null)
            {                
                foreach (var participant in dayExpenses.ParticipantsList)
                {
                    optionList.Add(new SelectListItem { Text = participant, Value = participant });
                }                
            }

            return new SelectList(optionList, "Value", "Text");
        }

        public async Task<Check> AddCheckRCheck(Check check)
        {
            var dayExpenses = await GetDayExpensesWithCheck(check.DayExpensesId);

            return await _checkRepository.Insert(check);
        }

        public async Task<DayExpenses> AddCheck(Check check)
        {
            var dayExpenses = await GetDayExpensesWithCheck(check.DayExpensesId);

            await _checkRepository.Insert(check);

            return dayExpenses;
        }

        public async Task<Check> EditCheckRCheck(Check check)
        {
            return await _checkRepository.Update(check);
        }

        public async Task<DayExpenses> EditCheck(Check check)
        {
            await _checkRepository.Update(check);

            var dayExpenses = await GetDayExpensesWithCheck(check.DayExpensesId);
            return dayExpenses;
        }

        public async Task<DayExpenses> DeleteCheck(int id)
        {          
            var check = await _checkRepository.Delete(id);

            var dayExpenses = await GetDayExpensesWithCheck(check.DayExpensesId);
            return dayExpenses;
        }

        private async Task<DayExpenses> GetDayExpensesWithCheck(int dayExpensesId)
        {
            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var checks = await _checkRepository.GetAllDayChecks(dayExpensesId);
            dayExpenses.Checks = checks.ToList();

            return dayExpenses;
        }
    }
}
