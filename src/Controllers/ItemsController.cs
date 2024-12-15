using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using ExpensesCalculator.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: Items/CreateItem?dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> CreateItem(int checkId, int dayExpensesId)
        {            
            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetAllAvailableItemUsers(dayExpensesId);

            return PartialView("_CreateItem");
        }

        // GET: Items/EditItem/5?dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> EditItem(int? id, int dayExpensesId)
        {
            if (id is null)
                return NotFound();

            var item = await _itemService.GetItemById((int)id);

            if (item is null)
                return NotFound();

            ViewData["CheckId"] = item.CheckId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetCheckedItemUsers(item, dayExpensesId);
            ViewData["FormatUserList"] = await _itemService.GetItemUsers(item.UsersList);

            return PartialView("_EditItem", item);
        }

        // GET: Items/DeleteItem/5?dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id is null)
                return NotFound();

            var item = await _itemService.GetItemById((int)id);

            if (item is null)
                return NotFound();

            ViewData["FormatUserList"] = await _itemService.GetItemUsers(item.UsersList);

            return PartialView("_DeleteItem", item);
        }

        // POST: Items/Create?dayExpensesId=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CheckId,UserList,Name,Description,Price,Amount,Id")] AddItemViewModel<int> newItem, int dayExpensesId)
        {
            if (ModelState.IsValid)
            {
                var model = await _itemService.AddItem(newItem);
                return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", model);
            }
            
            ViewData["CheckId"] = newItem.CheckId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetCheckedItemUsers(newItem, dayExpensesId);
            ViewData["FormatUserList"] = await _itemService.GetItemUsers(newItem.UserList);

            return PartialView("_CreateItem", newItem);
        }

        // POST: Items/Edit/5?checkId=1&dayExpensesId=2
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CheckId,UsersList,Name,Description,Price,Id")] Item item, int dayExpensesId)
        {
            item = await _itemService.SetCheck(item);
            ModelState.Clear();

            if (item.UsersList.First() is null)
                ModelState.AddModelError("UsersList", "Choose some users");
            if (item.Price <= 0)
                ModelState.AddModelError("Price", "Enter correct price");

            if (TryValidateModel(item))
            {
                var model = await _itemService.EditItem(item);

                return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", model);
            }

            if (item.UsersList.First() is not null)
                item.UsersList = item.UsersList.First().Split(',');

            ViewData["CheckId"] = item.CheckId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetCheckedItemUsers(item, dayExpensesId);
            ViewData["FormatUserList"] = await _itemService.GetItemUsers(item.UsersList);

            return PartialView("_EditItem", item);
        }

        // POST: Items/Delete/5?dayExpensesId=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _itemService.DeleteItem(id);
            return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", model);
        }
    }
}
