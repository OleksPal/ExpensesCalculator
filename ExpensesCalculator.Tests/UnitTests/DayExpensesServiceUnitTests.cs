using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using ExpensesCalculator.Services;

namespace ExpensesCalculator.UnitTests
{
    public class DayExpensesServiceUnitTests
    {
        private readonly IDayExpensesService _dayExpensesService;

        public DayExpensesServiceUnitTests() 
        {
            _dayExpensesService = Helper.GetRequiredService<IDayExpensesService>()
                ?? throw new ArgumentNullException(nameof(IDayExpensesService));
        }

        [Fact]
        public async void GetAllDaysForDefaultUser()
        {
            var emptyDayCollection = new List<DayExpenses>();

            var dayCollection = await _dayExpensesService.GetAllDays();

            Assert.Equal(emptyDayCollection, dayCollection);
        }
    }
}