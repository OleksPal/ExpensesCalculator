using ExpensesCalculator.Models;
using ExpensesCalculator.Services;

namespace ExpensesCalculator.UnitTests
{
    public class ItemServiceUnitTests
    {
        private readonly IItemService _itemService;
        private readonly Item _itemDefaultObject = new Item
        {
            Name = "Item1",
            Description = "Description1",
            Price = 1000,
            CheckId = 1,
            UsersList = ["User1", "User2"]
        };

        public ItemServiceUnitTests() 
        {
            _itemService = Helper.GetRequiredService<IItemService>()
                ?? throw new ArgumentNullException(nameof(IItemService));
        }

        #region SetCheck method
        [Fact]
        public async void SetCheckThatDoesNotExists()
        {
            var item = _itemDefaultObject;
            item.CheckId = 0;

            await _itemService.SetCheck(item);

            Assert.Null(item.Check);
        }

        [Fact]
        public async void SetCheckThatExists()
        {
            var item = _itemDefaultObject;

            await _itemService.SetCheck(item);

            Assert.Equal(1, item.Check.Id);
        }
        #endregion

        #region GetItemById method
        [Fact]
        public async void GetItemByIdThatDoesNotExists()
        {
            var item = await _itemService.GetItemById(5);

            Assert.Null(item);
        }

        [Fact]
        public async void GetItemByIdThatExists()
        {
            var item = await _itemService.GetItemById(1);

            Assert.Equal("Item1", item.Name);
        }
        #endregion

        #region GetItemUsers method
        [Fact]
        public async void GetNullItemUsers()
        {
            List<string> userList = null;

            Func<Task> act = () => _itemService.GetItemUsers(userList);

            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async void GetItemUsers()
        {
            var userList = _itemDefaultObject.UsersList;

            var userListSting = await _itemService.GetItemUsers(userList);

            Assert.Equal("User1, User2", userListSting);
        }
        #endregion
    }
}