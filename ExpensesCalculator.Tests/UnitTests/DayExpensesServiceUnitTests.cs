using ExpensesCalculator.Models;
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

        [Fact]
        public async void GetAllDaysForUser()
        {
            _dayExpensesService.RequestorName = "123@g.c";
            var emptyDayCollection = new List<DayExpenses>();

            var dayCollection = await _dayExpensesService.GetAllDays();

            Assert.Equal(emptyDayCollection, dayCollection);
        }
    }
}