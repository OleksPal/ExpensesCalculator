using ExpensesCalculator.Models;
using ExpensesCalculator.Services;

namespace ExpensesCalculator.UnitTests
{
    public class DayExpensesServiceUnitTests
    {
        private readonly IDayExpensesService _dayExpensesService;
        private readonly List<DayExpenses> _emptyDayExpensesList = new List<DayExpenses>();
        private readonly DayExpenses _dayExpensesDefaultObject = new DayExpenses
        {
            Date = new DateOnly(2024, 1, 1),
            ParticipantsList = ["User1", "User2"],
            PeopleWithAccessList = ["Guest"]
        };

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

            var dayCollection = await _dayExpensesService.GetAllDays();

            Assert.Equal(_emptyDayExpensesList, dayCollection);
        }
        #endregion

        #region GetDayExpensesById method
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

            Assert.Equal(_dayExpensesDefaultObject.Date, dayExpenses.Date);
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
            var dayExpenses = await _dayExpensesService.GetDayExpensesByIdWithChecks(1);

            Assert.Equal(_dayExpensesDefaultObject.Date, dayExpenses.Date);
        }
        #endregion

        #region GetFullDayExpensesById method
        [Fact]
        public async void GetFullDayExpensesByIdThatDoesNotExists()
        {
            var dayExpenses = await _dayExpensesService.GetFullDayExpensesById(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void GetFullDayExpensesByIdThatExists()
        {
            var dayExpenses = await _dayExpensesService.AddDayExpenses(_dayExpensesDefaultObject);
            dayExpenses = await _dayExpensesService.GetFullDayExpensesById(dayExpenses.Id);

            Assert.Equal(_dayExpensesDefaultObject.Date, dayExpenses.Date);
        }
        #endregion

        #region AddDayExpenses method
        [Fact]
        public async void AddNullDayExpenses()
        {
            DayExpenses dayExpenses = null;

            Func<Task> act = () => _dayExpensesService.AddDayExpenses(dayExpenses);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async void AddDayExpenses()
        {
            await _dayExpensesService.AddDayExpenses(_dayExpensesDefaultObject);
            var dayCollection = await _dayExpensesService.GetAllDays();

            Assert.Equal(2, dayCollection.Count);
        }
        #endregion

        #region EditDayExpenses method
        [Fact]
        public async void EditNullDayExpenses()
        {
            DayExpenses dayExpenses = null;

            Func<Task> act = () => _dayExpensesService.EditDayExpenses(dayExpenses);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async void EditDayExpenses()
        {
            var dayExpenses = await _dayExpensesService.AddDayExpenses(_dayExpensesDefaultObject);
            dayExpenses.Date = new DateOnly(2025, 12, 12);

            await _dayExpensesService.EditDayExpenses(dayExpenses);
            var updatedDayExpenses = await _dayExpensesService.GetDayExpensesById(dayExpenses.Id);

            Assert.Equal(dayExpenses, updatedDayExpenses);
        }
        #endregion

        #region DeleteDayExpenses method
        [Fact]
        public async void DeleteDayExpensesThatDoesNotExists()
        { 
            var dayExpenses = await _dayExpensesService.DeleteDayExpenses(5);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void DeleteDayExpensesThatExists()
        {
            await _dayExpensesService.DeleteDayExpenses(1);
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(1);

            Assert.Null(dayExpenses);
        }
        #endregion

        #region ChangeDayExpensesAccess method
        [Fact]
        public async void ChangeDayExpensesAccessForNullUser()
        {
            var response = await _dayExpensesService.ChangeDayExpensesAccess(1, null);

            Assert.Equal("There is no such user!", response);
        }

        [Fact]
        public async void ChangeDayExpensesAccessForDayExpensesThatDoesNotExists()
        {
            var response = await _dayExpensesService.ChangeDayExpensesAccess(5, "abc");

            Assert.Null(response);
        }
        #endregion
    }
}