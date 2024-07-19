using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> EditItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
                return NotFound();

            var item = await _itemService.GetItemById((int)id);

            if (item is null)
                return NotFound();

            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetCheckedItemUsers(item, dayExpensesId);
            ViewData["FormatUserList"] = await _itemService.GetItemUsers(item.Id);

            return PartialView("_EditItem", item);
        }

        // GET: Items/DeleteItem/5?dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> DeleteItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
                return NotFound();

            var item = await _itemService.GetItemById((int)id);

            if (item is null)
                return NotFound();

            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["FormatParticipantNames"] = await _itemService.GetItemUsers((int)id);

            return PartialView("_DeleteItem", item);
        }

        // POST: Items/Create?checkId=1&dayExpensesId=2
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CheckId,UsersList,Name,Description,Price,Id")] Item item, int dayExpensesId)
        {          
            item = await _itemService.SetCheck(item);
            ModelState.Clear();

            if (item.UsersList.First() is null)
                ModelState.AddModelError("UsersList", "Choose some users");

            if (TryValidateModel(item))
            {
                var model = await _itemService.AddItem(item);
                return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", model);
            }

            if(item.UsersList.First() is not null)
                item.UsersList = item.UsersList.First().Split(',');
            
            ViewData["CheckId"] = item.CheckId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetAllAvailableItemUsers(dayExpensesId);

            return PartialView("_CreateItem", item);
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

            return PartialView("_EditItem");
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
