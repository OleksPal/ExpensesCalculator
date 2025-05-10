using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.UnitTests
{
    public class ItemRepositoryTests : GenericRepositoryTests<Item>
    {
        public ItemRepositoryTests() : base() { }

        #region GetAllItems method
        [Fact]
        public async void GetAllItemsWhenCheckWithSuchIdDoesNotExists()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);

            var itemList = await repository.GetAllCheckItems(0);

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

        #region Update method
        public async override void UpdateValidObject()
        {
            using var context = CreateContext();
            var repository = new ItemRepository(context);
            var item = new Item();

            Func<Task> act = () => repository.Update(item);

            await Assert.ThrowsAsync<DbUpdateException>(act);
        }
        #endregion
    }
}
