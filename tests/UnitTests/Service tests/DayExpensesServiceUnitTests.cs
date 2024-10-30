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

            Assert.Contains(dayCollection, d => d.PeopleWithAccessList.Contains("Guest"));
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
            var dayExpenses = await _dayExpensesService.GetDayExpensesById(0);

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
            var dayExpenses = await _dayExpensesService.GetDayExpensesByIdWithChecks(0);

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
            var dayExpenses = await _dayExpensesService.GetFullDayExpensesById(0);

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
            var dayExpensesToAdd = _dayExpensesDefaultObject;

            await _dayExpensesService.AddDayExpenses(dayExpensesToAdd);
            var addedDayExpenses = await _dayExpensesService.GetDayExpensesById(dayExpensesToAdd.Id);

            Assert.Equal(dayExpensesToAdd.Id, addedDayExpenses.Id);
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
            var dayExpensesToAdd = _dayExpensesDefaultObject;
            await _dayExpensesService.AddDayExpenses(dayExpensesToAdd);
            var dayExpensesToEdit = await _dayExpensesService.GetDayExpensesById(dayExpensesToAdd.Id);
            var newDate = new DateOnly(2025, 12, 12);

            dayExpensesToEdit.Date = newDate;
            await _dayExpensesService.EditDayExpenses(dayExpensesToEdit);
            var editedDayExpenses = await _dayExpensesService.GetDayExpensesById(dayExpensesToAdd.Id);

            Assert.Equal(newDate, editedDayExpenses.Date);
        }
        #endregion

        #region DeleteDayExpenses method
        [Fact]
        public async void DeleteDayExpensesThatDoesNotExists()
        { 
            var dayExpenses = await _dayExpensesService.DeleteDayExpenses(0);

            Assert.Null(dayExpenses);
        }

        [Fact]
        public async void DeleteDayExpensesThatExists()
        {
            var dayExpensesToAdd = _dayExpensesDefaultObject;
            await _dayExpensesService.AddDayExpenses(dayExpensesToAdd);
            var dayExpensesToDelete = await _dayExpensesService.GetDayExpensesById(dayExpensesToAdd.Id);

            await _dayExpensesService.DeleteDayExpenses(dayExpensesToDelete.Id);
            var deletedDayExpenses = await _dayExpensesService.GetDayExpensesById(dayExpensesToAdd.Id);

            Assert.Null(deletedDayExpenses);
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
            var response = await _dayExpensesService.ChangeDayExpensesAccess(0, "abc");

            Assert.Null(response);
        }
        #endregion

        #region GetFormatParticipantsNames method
        [Fact]
        public async void GetFormatParticipantsNamesFromNull()
        {
            List<string> participantList = null;

            Func<Task> act = () => _dayExpensesService.GetFormatParticipantsNames(participantList);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async void GetFormatParticipantsNamesFromValidString()
        {
            List<string> participantList = ["Participant1", "Participant2", "Participant3"];
            var expenctedValue = "Participant1, Participant2, Participant3";

            var actualValue = await _dayExpensesService.GetFormatParticipantsNames(participantList);

            Assert.Equal(expenctedValue, actualValue);
        }
        #endregion

        #region GetCalculationForDayExpenses method
        [Fact]
        public async void GetCalculationForDayExpensesThatDoesNotExists()
        {
            var dayExpensesCalculation = await _dayExpensesService.GetCalculationForDayExpenses(0);

            Assert.Null(dayExpensesCalculation.AllUsersTrasactions);
        }
        #endregion
    }
}