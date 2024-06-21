using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ExpensesContext _context;
        private readonly ItemRepository repository;

        public ItemsController(ExpensesContext context)
        {
            _context = context;
            repository = new ItemRepository(context);
        }

        // GET: Items/CreateItem?checkId=1&dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> CreateItem(int checkId, int dayExpensesId)
        {
            var dayExpenses = await _context.Days.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dayExpensesId);
            if (dayExpenses is not null)
            {
                List<SelectListItem> optionList = new List<SelectListItem>();
                foreach (var participant in dayExpenses.Participants)
                {
                    optionList.Add(new SelectListItem { Text = participant, Value = participant, Selected = true });
                }
                ViewData["Participants"] = new MultiSelectList(optionList, "Value", "Text");
            }

            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;

            return PartialView("_CreateItem");
        }

        // GET: Items/EditItem/5?checkId=1&dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> EditItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await repository.GetById((int)id);

            if (item is null)
            {
                return NotFound();
            }

            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;

            var dayExpenses = await _context.Days.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dayExpensesId);
            if (dayExpenses is not null)
            {
                List<SelectListItem> optionList = new List<SelectListItem>();
                foreach (var participant in dayExpenses.Participants)
                {
                    optionList.Add(new SelectListItem { Text = participant, Value = participant, Selected = true });
                }
                ViewData["Participants"] = new MultiSelectList(optionList, "Value", "Text");
            }
            return PartialView("_EditItem", item);
        }

        // GET: Items/DeleteItem/5?checkId=1&dayExpensesId=2
        [HttpGet]
        public async Task<IActionResult> DeleteItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await repository.GetById((int)id);
            if (item is null)
            {
                return NotFound();
            }

            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;

            ViewData["FormatParticipantNames"] = GetFormatUserNames(item.Users);
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
                await repository.Insert(item, checkId);

                var check = await _context.Checks.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == checkId);

                var manager = new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
                return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", manager);
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
                    await repository.Update(item, checkId);

                    var check = await _context.Checks.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == checkId);

                    var manager = new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
                    return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", manager);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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

        // POST: Items/Delete/5?checkId=1&dayExpensesId=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int checkId, int dayExpensesId)
        {
            await repository.Delete(id, checkId);

            var check = await _context.Checks.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == checkId);

            var manager = new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
            return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", manager);
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        private string GetFormatUserNames(List<string> participants)
        {
            string formatList = String.Empty;
            for (int i = 0; i < participants.Count; i++)
            {
                if (participants[i] is not null)
                {
                    formatList += participants[i];
                    if (i != participants.Count - 1)
                        formatList += ", ";
                }
            }
            return formatList;
        }
    }
}
