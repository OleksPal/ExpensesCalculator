using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.UnitTests
{
    public class CheckRepositoryTests : GenericRepositoryTests<Check>
    {
        public CheckRepositoryTests() : base() { }

        #region GetAllDayChecks method
        [Fact]
        public async void GetAllDayChecksWhenDayExpensesWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var checkList = await repository.GetAllDayChecks(5);

            Assert.Equal(emptyList, checkList);
        }

        [Fact]
        public async void GetAllDayChecksWhenDayExpensesWithSuchIdExists()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var checkList = await repository.GetAllDayChecks(1);

            Assert.Equal("Shop1", checkList.First().Location);
        }
        #endregion

        #region GetById method
        [Fact]
        public override async void GetByIdWhenItemWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var check = await repository.GetById(5);

            Assert.Null(check);
        }

        [Fact]
        public override async void GetByIdWhenItemWithSuchIdExists()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var check = await repository.GetById(1);

            Assert.Equal("Shop1", check.Location);
        }
        #endregion

        #region Update method
        public async override void UpdateValidObject()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);
            var check = new Check();

            Func<Task> act = () => repository.Update(check);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }
        #endregion
    }
}
