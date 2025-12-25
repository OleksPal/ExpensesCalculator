using ExpensesCalculator.Mappers;
using ExpensesCalculator.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using ExpensesCalculator.Services.Interfaces;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class ItemsController : LanguageController
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<JsonResult> GetItemById(int id)
        {
            var item = await _itemService.GetById(id);

            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return Json(item, options);
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

            var item = await _itemService.GetById((int)id);

            if (item is null)
                return NotFound();

            ViewData["CheckId"] = item.CheckId;
            ViewData["DayExpensesId"] = dayExpensesId;

            var editItemViewModel = item.ToEditItemViewModel();
            return PartialView("_EditItem", editItemViewModel);
        }

        // GET: Items/DeleteItem/5
        [HttpGet]
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id is null)
                return NotFound();

            var item = await _itemService.GetById((int)id);

            if (item is null)
                return NotFound();

            return PartialView("_DeleteItem", item);
        }

        // POST: Items/Create?dayExpensesId=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CheckId,UserList,Name,Description,Price,Amount,Id")] AddItemViewModel<int> newItem, int dayExpensesId)
        {
            ViewData["CheckId"] = newItem.CheckId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetCheckedItemUsers(newItem.UserList, dayExpensesId);

            return PartialView("_CreateItem", newItem);
        }

        // POST: Items/Edit/5?checkId=1&dayExpensesId=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CheckId,UserList,Name,Description,Price,Amount,Id")] EditItemViewModel<int> item, int dayExpensesId)
        {
            ViewData["CheckId"] = item.CheckId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetCheckedItemUsers(item.UserList, dayExpensesId);

            return PartialView("_EditItem", item);
        }

        // POST: Items/Delete/5?dayExpensesId=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _itemService.DeleteItem(id);
            return PartialView("~/Views/Checks/_ManageCheckItems.cshtml");
        }
    }
}
