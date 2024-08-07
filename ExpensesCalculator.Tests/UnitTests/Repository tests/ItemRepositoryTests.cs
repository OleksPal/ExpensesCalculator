using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;

namespace ExpensesCalculator.UnitTests
{
    public class ItemRepositoryTests : GenericRepositoryTests<Item>
    {
        public ItemRepositoryTests() : base() { }

        #region GetAllItems method
        [Fact]
        public async void GetAllItemsWhenCheckIdIsZero()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context); 

            var itemList = await repository.GetAllCheckItems(0);

            Assert.Equal(emptyList, itemList);
        }

        [Fact]
        public async void GetAllItemsWhenCheckIdIsNegative()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var itemList = await repository.GetAllCheckItems(-1);

            Assert.Equal(emptyList, itemList);
        }

        [Fact]
        public async void GetAllItemsWhenCheckWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var itemList = await repository.GetAllCheckItems(5);

            Assert.Equal(emptyList, itemList);
        }

        [Fact]
        public async void GetAllItemsWhenCheckWithSuchIdExists()
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
        public async void GetByIdWhenItemWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var item = await repository.GetById(5);

            Assert.Null(item);
        }

        [Fact]
        public async void GetByIdWhenItemWithSuchIdExists()
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
        public async void GetItemPriceByIdWhenItemWithSuchDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var price = await repository.GetItemPriceById(5);

            Assert.Equal(0, price);
        }

        [Fact]
        public async void GetItemPriceByIdWhenItemWithSuchIdExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var price = await repository.GetItemPriceById(1);

            Assert.Equal(1000m, price);
        }
        #endregion
    }
}
