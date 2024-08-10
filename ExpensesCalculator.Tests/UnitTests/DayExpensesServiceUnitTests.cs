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

        [Fact]
        public async void GetDayExpensesByIdThatDoesNotExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void GetDayExpensesByIdThatExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(1);

            Assert.NotNull(dayExpenses);
        }

        [Fact]
        public async void GetDayExpensesByIdWithChecksThatDoesNotExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void GetDayExpensesByIdWithChecksThatExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(1);

            Assert.NotNull(dayExpenses);
        }

        [Fact]
        public async void GetFullDayExpensesByIdThatDoesNotExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void GetFullDayExpensesByIdThatExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(1);

            Assert.NotNull(dayExpenses);
        }
    }
}