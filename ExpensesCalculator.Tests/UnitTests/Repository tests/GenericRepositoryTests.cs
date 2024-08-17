using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ExpensesCalculator.UnitTests
{
    public abstract class GenericRepositoryTests<T> : IDisposable where T : DbObject, new()
    {
        protected readonly DbConnection _connection;
        protected readonly DbContextOptions<ExpensesContext> _contextOptions;

        protected readonly List<T> emptyList = new List<T>();

        public GenericRepositoryTests()
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

        public ExpensesContext CreateContext() => new ExpensesContext(_contextOptions);

        public void Dispose() => _connection.Dispose();

        #region GetAll method
        [Fact]
        public virtual async void GetAll()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);

            var list = await repository.GetAll();

            Assert.NotEqual(emptyList, list);
        }
        #endregion

        #region GetById method
        [Fact]
        public virtual async void GetByIdWhenItemWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);

            var obj = await repository.GetById(0);

            Assert.Null(obj);
        }

        [Fact]
        public virtual async void GetByIdWhenItemWithSuchIdExists()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);

            var obj = await repository.GetById(1);

            Assert.NotNull(obj);
        }
        #endregion

        #region Insert method
        [Fact]
        public virtual async void InsertValidObjectWithDefaultValues()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);
            T newObject = new T();

            Func<Task> act = () => repository.Insert(newObject);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public virtual async void InsertNull()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);
            T newObject = null;

            Func<Task> act = () => repository.Insert(newObject);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }
        #endregion

        #region Update method
        [Fact]
        public virtual async void UpdateNull()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);
            T newObject = null;

            Func<Task> act = () => repository.Update(newObject);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        [Fact]
        public virtual async void UpdateInvalidObject()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);
            T newObject = new T();

            Func<Task> act = () => repository.Update(newObject);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }

        [Fact]
        public abstract void UpdateValidObject();
        #endregion

        #region Delete method
        [Fact]
        public virtual async void DeleteRecordThatDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);

            var deletedItem = await repository.Delete(0);

            Assert.Null(deletedItem);
        }

        [Fact]
        public virtual async void DeleteRecordThatExists()
        {
            using var context = CreateContext();
            var repository = new GenericRepository<T>(context);

            var deletedItem = await repository.Delete(1);

            Assert.NotNull(deletedItem);
        }
        #endregion
    }
}
