using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ExpensesCalculator.UnitTests
{
    public class ItemRepositoryTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<ExpensesContext> _contextOptions;

        private readonly List<Item> emptyItemList = new List<Item>();

        public ItemRepositoryTests()
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

        #region GetAllItems method
        [Fact]
        public async void GetAllItemsWhenCheckIdIsZero()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context); 

            var itemList = await repository.GetAllCheckItems(0);

            Assert.Equal(emptyItemList, itemList);
        }

        [Fact]
        public async void GetAllItemsWhenCheckIdIsNegative()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var itemList = await repository.GetAllCheckItems(-1);

            Assert.Equal(emptyItemList, itemList);
        }

        [Fact]
        public async void GetAllItemsWhenCheckWithSuchIdIsNoExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var itemList = await repository.GetAllCheckItems(5);

            Assert.Equal(emptyItemList, itemList);
        }

        [Fact]
        public async void GetAllItemsWhenCheckWithSuchIdIsExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var itemList = await repository.GetAllCheckItems(1);

            Assert.Equal("Item1", itemList.First().Name);
        }
        #endregion

        #region GetById method
        [Fact]
        public async void GetByIdWhenIdIsZero()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var item = await repository.GetById(0);

            Assert.Null(item);
        }

        [Fact]
        public async void GetByIdWhenIdIsNegative()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var item = await repository.GetById(-1);

            Assert.Null(item);
        }

        [Fact]
        public async void GetByIdWhenItemWithSuchIdIsNoExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var item = await repository.GetById(5);

            Assert.Null(item);
        }

        [Fact]
        public async void GetByIdWhenItemWithSuchIdIsExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var item = await repository.GetById(1);

            Assert.Equal("Item1", item.Name);
        }
        #endregion

        #region GetItemPriceById method
        [Fact]
        public async void GetItemPriceByIdWhenIdIsZero()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var price = await repository.GetItemPriceById(0);

            Assert.Equal(0, price);
        }

        [Fact]
        public async void GetItemPriceByIdWhenIdIsNegative()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var price = await repository.GetItemPriceById(-1);

            Assert.Equal(0, price);
        }

        [Fact]
        public async void GetItemPriceByIdWhenItemWithSuchIdIsNoExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var price = await repository.GetItemPriceById(5);

            Assert.Equal(0, price);
        }

        [Fact]
        public async void GetItemPriceByIdWhenItemWithSuchIdIsExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var price = await repository.GetItemPriceById(1);

            Assert.Equal(1000m, price);
        }
        #endregion
    }
}
