using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;

namespace ExpensesCalculator.UnitTests
{
    public class DayExpensesRepositoryTests : GenericRepositoryTests<DayExpenses>
    {
        public DayExpensesRepositoryTests() : base() { }

        #region Insert method
        [Fact]
        public override async void InsertValidObjectWithDefaultValues()
        {
            using var context = CreateContext();
            var repository = new DayExpensesRepository(context);
            var dayExpenses = new DayExpenses();
            
            await repository.Insert(dayExpenses);
            var insertedValue = await repository.GetById(dayExpenses.Id);

            Assert.Equal(dayExpenses, insertedValue);
        }
        #endregion

        #region Update method
        [Fact]
        public override async void UpdateInvalidObject()
        {
            // All range of values for every property of the DayExpenses object are valid
        }

        [Fact]
        public async override void UpdateValidObject()
        {
            using var context = CreateContext();
            var repository = new DayExpensesRepository(context);
            var dayExpenses = new DayExpenses();

            await repository.Update(dayExpenses);
            var updatedExpenses = await repository.GetById(dayExpenses.Id);

            Assert.Equal(dayExpenses, updatedExpenses);
        }
        #endregion
    }
}
