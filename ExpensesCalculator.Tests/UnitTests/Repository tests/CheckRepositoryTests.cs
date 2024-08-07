using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;

namespace ExpensesCalculator.UnitTests
{
    public class CheckRepositoryTests : GenericRepositoryTests<Check>
    {
        public CheckRepositoryTests() : base() { }

        #region GetAllDayChecks method
        [Fact]
        public async void GetAllDayChecksWhenDayExpensesIdIsZero()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context); 

            var checkList = await repository.GetAllDayChecks(0);

            Assert.Equal(emptyList, checkList);
        }

        [Fact]
        public async void GetAllDayChecksWhenDayExpensesIdIsNegative()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var checkList = await repository.GetAllDayChecks(-1);

            Assert.Equal(emptyList, checkList);
        }

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
        public async void GetByIdWhenIdIsZero()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var check = await repository.GetById(0);

            Assert.Null(check);
        }

        [Fact]
        public async void GetByIdWhenIdIsNegative()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var check = await repository.GetById(-1);

            Assert.Null(check);
        }

        [Fact]
        public async void GetByIdWhenItemWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var check = await repository.GetById(5);

            Assert.Null(check);
        }

        [Fact]
        public async void GetByIdWhenItemWithSuchIdExists()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var check = await repository.GetById(1);

            Assert.Equal("Shop1", check.Location);
        }
        #endregion
    }
}
