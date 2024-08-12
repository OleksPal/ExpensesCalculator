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

        #region GetAllDays method
        [Fact]
        public async void GetAllDaysForDefaultUser()
        {
            var dayCollection = await _dayExpensesService.GetAllDays();

            Assert.Single(dayCollection);
        }

        [Fact]
        public async void GetAllDaysForUser()
        {
            _dayExpensesService.RequestorName = "123@g.c";
            var emptyDayCollection = new List<DayExpenses>();

            var dayCollection = await _dayExpensesService.GetAllDays();

            Assert.Equal(emptyDayCollection, dayCollection);
        }
        #endregion

        #region GetDayExpensesById
        [Fact]
        public async void GetDayExpensesByIdThatDoesNotExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void GetDayExpensesByIdThatExists()
        {
            var dayExpensesDate = new DateOnly(2024, 1, 1);

            var dayExpenses = await _dayExpensesService.GetDayExpensesById(1);

            Assert.Equal(dayExpensesDate, dayExpenses.Date);
        }
        #endregion

        #region GetDayExpensesByIdWithChecks method
        [Fact]
        public async void GetDayExpensesByIdWithChecksThatDoesNotExists()
        {
            var dayExpenses = await _dayExpensesService.GetDayExpensesByIdWithChecks(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void GetDayExpensesByIdWithChecksThatExists()
        {
            var dayExpensesDate = new DateOnly(2024, 1, 1);

            var dayExpenses = await _dayExpensesService.GetDayExpensesByIdWithChecks(1);

            Assert.Equal(dayExpensesDate, dayExpenses.Date);
        }
        #endregion

        #region GetFullDayExpensesById
        [Fact]
        public async void GetFullDayExpensesByIdThatDoesNotExists()
        {
            var dayExpenses = await _dayExpensesService.GetFullDayExpensesById(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void GetFullDayExpensesByIdThatExists()
        {
            var dayExpensesDate = new DateOnly(2024, 1, 1);

            var dayExpenses = await _dayExpensesService.GetFullDayExpensesById(1);

            Assert.Equal(dayExpensesDate, dayExpenses.Date);
        }
        #endregion
    }
}