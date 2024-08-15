using ExpensesCalculator.Models;
using ExpensesCalculator.Services;

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
        public async void SetDayExpensesWithDayExpensesThatDoesNotExists()
        {
            var check = _checkDefaultObject;
            check.DayExpensesId = 0;

            await _checkService.SetDayExpenses(check);

            Assert.Null(check.DayExpenses);
        }

        [Fact]
        public async void SetDayExpensesWithDayExpensesThatExists()
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
    }
}