﻿using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

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

        public async Task<Check> GetCheckById(int id)
        {
            return await _checkRepository.GetById(id);
        }

        public async Task<Check> GetCheckByIdWithItems(int id)
        {
            var check = await GetCheckById(id);
            check.Items.AddRange(await _itemRepository.GetAllCheckItems(id));

            return check;
        }

        public async Task<bool> CheckExists(int id)
        {
            return await GetCheckById(id) is not null;
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

        public async Task<DayExpenses> AddCheck(Check check, int dayExpensesId)
        {
            var dayExpenses = await GetDayExpensesWithCheck(dayExpensesId);

            await _checkRepository.Insert(check);

            return dayExpenses;
        }

        public async Task<DayExpenses> EditCheck(Check check, int dayExpensesId)
        {
            await _checkRepository.Update(check);

            var dayExpenses = await GetDayExpensesWithCheck(dayExpensesId);
            return dayExpenses;
        }

        public async Task<DayExpenses> DeleteCheck(int id, int dayExpensesId)
        {          
            await _checkRepository.Delete(id);

            var dayExpenses = await GetDayExpensesWithCheck(dayExpensesId);
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
