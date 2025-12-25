using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using ExpensesCalculator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public class CheckService : ICheckService
    {
        private readonly ICheckRepository _checkRepository;
        private readonly IDayExpensesRepository _dayExpensesRepository;
        
        public CheckService(ICheckRepository checkRepository, IDayExpensesRepository dayExpensesRepository)
        {
            _checkRepository = checkRepository;
            _dayExpensesRepository = dayExpensesRepository;
        }

        public async Task<Check> GetById(int id)
        {
            return await _checkRepository.GetById(id);
        }

        public async Task<SelectList> GetAllAvailableCheckPayers(int dayExpensesId)
        {
            var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesId);
            var optionList = new List<SelectListItem>();

            if (dayExpenses is not null)
            {                
                foreach (var participant in dayExpenses.Participants)
                {
                    optionList.Add(new SelectListItem { Text = participant, Value = participant });
                }                
            }

            return new SelectList(optionList, "Value", "Text");
        }

        public async Task AddCheck(Check check)
        {
            await _checkRepository.Insert(check);
        }

        public async Task EditCheck(Check check)
        {
            await _checkRepository.Update(check);
        }

        public async Task DeleteCheck(int id)
        {          
            await _checkRepository.Delete(id);
        }
    }
}
