using ExpensesCalculator.Mappers;
using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using ExpensesCalculator.ViewModels;

namespace ExpensesCalculator.UnitTests
{
    public class ItemServiceUnitTests
    {
        private readonly IItemService _itemService;
        private readonly Item _itemDefaultObject = new Item
        {
            Name = "Item2",
            Description = "Description2",
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
            var item = await _itemService.GetItemById(0);

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

            var itemUsers = await _itemService.GetItemUsers(userList);

            Assert.Null(itemUsers);
        }

        [Fact]
        public async void GetItemUsers()
        {
            var userList = _itemDefaultObject.UsersList;

            var userListSting = await _itemService.GetItemUsers(userList);

            Assert.Equal("User1, User2", userListSting);
        }
        #endregion

        #region GetAllAvailableItemUsers method
        [Fact]
        public async void GetAllAvailableItemUsersThatDoesNotExists()
        {
            var selectList = await _itemService.GetAllAvailableItemUsers(0);

            Assert.Empty(selectList.Items);
        }

        [Fact]
        public async void GetAllAvailableItemUsersThatExistsUsers()
        {
            var selectList = await _itemService.GetAllAvailableItemUsers(1);

            Assert.NotEmpty(selectList.Items);
        }
        #endregion

        #region GetCheckedItemUsers method
        [Fact]
        public async void GetCheckedItemUsersThatDoesNotExists()
        {
            var selectList = await _itemService.GetAllAvailableItemUsers(0);

            Assert.Empty(selectList.Items);
        }

        [Fact]
        public async void GetCheckedItemUsersThatExists()
        {
            var selectList = await _itemService.GetAllAvailableItemUsers(1);

            Assert.NotEmpty(selectList.Items);
        }
        #endregion

        #region AddItem method
        [Fact]
        public async void AddNullItem()
        {
            AddItemViewModel<int> itemToAdd = null;

            Func<Task> act = () => _itemService.AddItem(itemToAdd);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        //[Fact]
        //public async void AddItem()
        //{
        //    var addItemViewModel = _itemDefaultObject.ToAddItemViewModel();

        //    var check = await _itemService.AddItem(addItemViewModel);

        //    Assert.Single(check.Items);
        //}
        #endregion

        #region EditItem method
        [Fact]
        public async void EditNullItem()
        {
            EditItemViewModel<int> itemToEdit = null;

            Func<Task> act = () => _itemService.EditItem(itemToEdit);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        //[Fact]
        //public async void EditItem()
        //{
        //    var itemToAdd = _itemDefaultObject;

        //    Check check = await _itemService.AddItem(itemToAdd.ToAddItemViewModel());
        //    var itemToEdit = check.Items.First();
        //    itemToEdit.Name = "Name10";
        //    await _itemService.EditItem(itemToEdit.ToEditItemViewModel());
        //    var editedItem = await _itemService.GetItemById(itemToAdd.Id);

        //    Assert.Equal("Name10", editedItem.Name);
        //}
        #endregion

        #region DeleteItem method
        [Fact]
        public async void DeleteItemThatDoesNotExists()
        {
            Func<Task> act = () => _itemService.DeleteItem(0);

            await Assert.ThrowsAsync<NullReferenceException>(act);
        }

        //[Fact]
        //public async void DeleteCheckThatExists()
        //{
        //    var itemToAdd = _itemDefaultObject;

        //    await _itemService.AddItem(itemToAdd.ToAddItemViewModel());
        //    var itemToDelete = await _itemService.GetItemById(itemToAdd.Id);
        //    await _itemService.DeleteItem(itemToDelete.Id);
        //    var deletedCheck = await _itemService.GetItemById(itemToAdd.Id);

        //    Assert.Null(deletedCheck);
        //}
        #endregion
    }
}