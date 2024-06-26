using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            ViewData["Participants"] = await _itemService.GetAllAvailableItemUsers(dayExpensesId);
            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;

            return PartialView("_CreateItem");
        }

        // GET: Items/EditItem/5?dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> EditItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _itemService.GetItemById((int)id);

            if (item is null)
            {
                return NotFound();
            }

            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;
            ViewData["Participants"] = await _itemService.GetAllAvailableItemUsers(dayExpensesId);

            return PartialView("_EditItem", item);
        }

        // GET: Items/DeleteItem/5?dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> DeleteItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _itemService.GetItemById((int)id);

            if (item is null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("Users,Name,Description,Price,Id")] Item item, int checkId, int dayExpensesId)
        {
            if (ModelState.IsValid)
            {
                var model = await _itemService.AddItem(item, checkId, dayExpensesId);
                return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", model);
            }

            return PartialView("_CreateItem");
        }

        // POST: Items/Edit/5?checkId=1&dayExpensesId=2
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Users,Name,Description,Price,Id")] Item item, int checkId, int dayExpensesId)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {                
                try
                {
                    var model = await _itemService.EditItem(item, checkId, dayExpensesId);
                    return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _itemService.ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return PartialView("_EditItem");
        }

        // POST: Items/Delete/5?dayExpensesId=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int checkId, int dayExpensesId)
        {
            var model = await _itemService.DeleteItem(id, checkId, dayExpensesId);
            return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", model);
        }
    }
}
