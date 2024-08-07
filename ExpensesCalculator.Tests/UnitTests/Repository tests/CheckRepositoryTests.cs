using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ExpensesCalculator.UnitTests
{
    public class CheckRepositoryTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<ExpensesContext> _contextOptions;

        private readonly List<Check> emptyCheckList = new List<Check>();

        public CheckRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ExpensesContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new ExpensesContext(_contextOptions);

            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }

        ExpensesContext CreateContext() => new ExpensesContext(_contextOptions);

        public void Dispose() => _connection.Dispose();

        #region GetAllDayChecks method
        [Fact]
        public async void GetAllDayChecksWhenDayExpensesIdIsZero()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context); 

            var checkList = await repository.GetAllDayChecks(0);

            Assert.Equal(emptyCheckList, checkList);
        }

        [Fact]
        public async void GetAllDayChecksWhenDayExpensesIdIsNegative()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var checkList = await repository.GetAllDayChecks(-1);

            Assert.Equal(emptyCheckList, checkList);
        }

        [Fact]
        public async void GetAllDayChecksWhenDayExpensesWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new CheckRepository(context);

            var checkList = await repository.GetAllDayChecks(5);

            Assert.Equal(emptyCheckList, checkList);
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
