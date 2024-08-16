using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.UnitTests
{
    public class CheckServiceUnitTests
    {
        private readonly ICheckService _checkService;
        private readonly Check _checkDefaultObject = new Check
        {
            Location = "Shop1",
            Sum = 1000,
            Payer = "User1",
            DayExpensesId = 1
        };

        public CheckServiceUnitTests() 
        {
            _checkService = Helper.GetRequiredService<ICheckService>()
                ?? throw new ArgumentNullException(nameof(ICheckService));
        }

        #region SetDayExpenses method
        [Fact]
        public async void SetDayExpensesThatDoesNotExists()
        {
            var check = _checkDefaultObject;
            check.DayExpensesId = 0;

            await _checkService.SetDayExpenses(check);

            Assert.Null(check.DayExpenses);
        }

        [Fact]
        public async void SetDayExpensesThatExists()
        {
            var check = _checkDefaultObject;

            await _checkService.SetDayExpenses(check);

            Assert.Equal(1, check.DayExpenses.Id);
        }
        #endregion

        #region GetCheckById method
        [Fact]
        public async void GetCheckByIdThatDoesNotExists()
        {
            var check = await _checkService.GetCheckById(5);

            Assert.Null(check);
        }

        [Fact]
        public async void GetCheckByIdThatExists()
        {
            var check = await _checkService.GetCheckById(1);

            Assert.Equal("Shop1", check.Location);
        }
        #endregion

        #region GetCheckByIdWithItems method
        [Fact]
        public async void GetCheckByIdWithItemsThatDoesNotExists()
        {
            var check = await _checkService.GetCheckByIdWithItems(5);

            Assert.Null(check);
        }

        [Fact]
        public async void GetCheckByIdWithItemsThatExists()
        {
            var check = await _checkService.GetCheckByIdWithItems(1);

            Assert.NotNull(check.Items);
        }
        #endregion

        #region GetAllAvailableCheckPayers method
        [Fact]
        public async void GetAllAvailableCheckThatDoesNotExistsPayers()
        {
            var selectList = await _checkService.GetAllAvailableCheckPayers(5);

            Assert.Empty(selectList.Items);
        }

        [Fact]
        public async void GetAllAvailableCheckThatExistsPayers()
        {
            var selectList = await _checkService.GetAllAvailableCheckPayers(1);

            Assert.NotEmpty(selectList.Items);
        }
        #endregion

        #region AddCheck method
        [Fact]
        public async void AddNullCheck()
        {
            Check checkToAdd = null;

            Func<Task> act = () => _checkService.AddCheck(checkToAdd);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async void AddCheck()
        {
            var checkToAdd = _checkDefaultObject;

            await _checkService.AddCheck(checkToAdd);
            var addedDayExpenses = await _checkService.GetCheckById(checkToAdd.Id);

            Assert.Equal(checkToAdd.Id, addedDayExpenses.Id);
        }
        #endregion

        #region EditCheck method
        [Fact]
        public async void EditNullCheck()
        {
            Check checkToEdit = null;

            Func<Task> act = () => _checkService.EditCheck(checkToEdit);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async void EditCheck()
        {
            var checkToAdd = _checkDefaultObject;

            await _checkService.AddCheck(checkToAdd);
            var checkToEdit = await _checkService.GetCheckById(checkToAdd.Id);
            checkToEdit.Location = "Shop10";
            await _checkService.EditCheck(checkToEdit);
            var editedCheck = await _checkService.GetCheckById(checkToAdd.Id);

            Assert.Equal("Shop10", editedCheck.Location);
        }
        #endregion

        #region DeleteCheck method
        [Fact]
        public async void DeleteCheckThatDoesNotExists()
        {
            Func<Task> act = () => _checkService.DeleteCheck(5);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public async void DeleteCheckThatExists()
        {
            var checkToAdd = _checkDefaultObject;

            await _checkService.AddCheck(checkToAdd);
            var checkToDelete = await _checkService.GetCheckById(checkToAdd.Id);
            await _checkService.DeleteCheck(checkToDelete.Id);
            var deletedCheck = await _checkService.GetCheckById(checkToAdd.Id);

            Assert.Null(deletedCheck);
        }
        #endregion
    }
}