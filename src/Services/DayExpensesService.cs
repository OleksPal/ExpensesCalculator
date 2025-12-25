using ExpensesCalculator.Extensions;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using ExpensesCalculator.ViewModels;

namespace ExpensesCalculator.Services
{
    public class DayExpensesService : IDayExpensesService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICheckRepository _checkRepository;
        private readonly IDayExpensesRepository _dayExpensesRepository;
        private readonly IUserRepository _userRepository;

        public string RequestorName { get; set; } = "Guest";

        public DayExpensesService(IItemRepository itemRepository, 
            ICheckRepository checkRepository, IDayExpensesRepository dayExpensesRepository, 
            IUserRepository userRepository)
        {
            _itemRepository = itemRepository;
            _checkRepository = checkRepository;
            _dayExpensesRepository = dayExpensesRepository;
            _userRepository = userRepository;
        }

        public async Task<ICollection<DayExpensesViewModel>> GetAllDays()
        {            
            var dayExpenses = await _dayExpensesRepository.GetAll();
            dayExpenses = dayExpenses.Where(r => r.PeopleWithAccess.Contains(RequestorName)).ToList();

            var dayExpensesViewModels = new List<DayExpensesViewModel>();
            foreach (var dayExpense in dayExpenses)
            {
                var checks = await _checkRepository.GetAllDayChecks(dayExpense.Id);
                var totalSum = 0m;

                foreach (var check in checks)
                {
                    var items = await _itemRepository.GetAllCheckItems(check.Id);
                    totalSum += items.Select(item => item.Price).Sum();
                }

                dayExpensesViewModels.Add(
                    new DayExpensesViewModel
                    {
                        DayExpenses = dayExpense,
                        TotalSum = totalSum
                    }
                );
            }

            return dayExpensesViewModels
                .Where(r => r.DayExpenses.PeopleWithAccess.Contains(RequestorName)).ToList();
        }

        public async Task<DayExpensesViewModel> GetDayExpensesViewModelById(int id)
        {
            var dayExpenses = await GetById(id);
            var checks = await _checkRepository.GetAllDayChecks(dayExpenses.Id);
            var totalSum = 0m;

            foreach (var check in checks)
            {
                var items = await _itemRepository.GetAllCheckItems(check.Id);
                totalSum += items.Select(item => item.Price).Sum();
            }

            var dayExpensesViewModel = new DayExpensesViewModel
            {
                DayExpenses = dayExpenses,
                TotalSum = totalSum
            };

            return dayExpensesViewModel;
        }

        public async Task<DayExpenses> GetById(int id)
        {
            var dayExpenses = await _dayExpensesRepository.GetById(id);
            
            if (dayExpenses == null || !dayExpenses.PeopleWithAccess.Contains(RequestorName))
                return null;

            return dayExpenses;
        }

        public async Task AddDayExpenses(DayExpenses dayExpenses)
        {
            await _dayExpensesRepository.Insert(dayExpenses);
        }

        public async Task EditDayExpenses(DayExpenses dayExpenses)
        {
            if (dayExpenses.PeopleWithAccess.Contains(RequestorName))
                dayExpenses = await _dayExpensesRepository.Update(dayExpenses);
        }

        public async Task DeleteDayExpenses(int id)
        {
            var dayExpensesToDelete = await GetById(id);

            if (dayExpensesToDelete is not null)
                dayExpensesToDelete = await _dayExpensesRepository.Delete(id);;
        }

        public async Task<DayExpensesCalculationViewModel> GetCalculationForDayExpenses(int id)
        {
            var dayExpenses = await GetById(id);
            return dayExpenses.GetCalculations();
        }

        public async Task<string?> ChangeDayExpensesAccess(int id, string newUserWithAccess)
        {
            var dayExpenses = await GetById(id);

            if (dayExpenses is not null)
            {
                bool isUserExist = _userRepository.UserExists(newUserWithAccess);

                if (!isUserExist)
                    return "There is no such user!";
                else if (dayExpenses.PeopleWithAccess.Contains(newUserWithAccess))
                    return "This user already has access!";
                else
                {
                    dayExpenses.PeopleWithAccess.Add(newUserWithAccess);
                    await _dayExpensesRepository.Update(dayExpenses);
                    return "Done!";
                }
            }

            return null;
        }      
    }
}
