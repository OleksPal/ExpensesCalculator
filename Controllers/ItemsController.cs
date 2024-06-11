using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ExpensesContext _context;

        public ItemsController(ExpensesContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> EditItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
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

        public async Task<IActionResult> DeleteItem(int? id, int checkId, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item is null)
            {
                return NotFound();
            }

            ViewData["CheckId"] = checkId;
            ViewData["DayExpensesId"] = dayExpensesId;

            ViewData["FormatParticipantNames"] = GetFormatUserNames(item.Users);
            return PartialView("_DeleteItem", item);
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Users,Name,Description,Price,Id")] Item item, int checkId, int dayExpensesId)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.Checks.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == checkId);

                if (check is null)
                {
                    return NotFound();
                }

                _context.Items.Add(item);
                check.Items.Add(item);
                check.Sum += item.Price;
                await _context.SaveChangesAsync();

                var manager = new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
                return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", manager);
            }
            return PartialView("_CreateItem");
        }

        // POST: Items/Edit/5
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
                    var check = await _context.Checks.Include(c => c.Items).AsNoTracking()                    
                        .FirstOrDefaultAsync(m => m.Id == checkId);

                    if (check is null)
                    {
                        return NotFound();
                    }

                    var oldItem = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);

                    if (oldItem is null)
                    {
                        return NotFound();
                    }

                    var itemToReplace = check.Items.Find(i => i.Id == oldItem.Id);
                    if(itemToReplace is not null)
                    {
                        check.Items.Remove(itemToReplace);
                        check.Items.Add(item);
                    }

                    _context.Items.Update(item);
                    await ChangeCheckSum(checkId, item.Price - oldItem.Price);

                    await _context.SaveChangesAsync();

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

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int checkId, int dayExpensesId)
        {
            var item = await _context.Items.FindAsync(id);
            if (item is not null)
            {
                var check = await _context.Checks.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == checkId);

                if (check is null)
                {
                    return NotFound();
                }

                check.Sum -= item.Price;
                _context.Items.Remove(item);

                await _context.SaveChangesAsync();

                var manager = new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
                return PartialView("~/Views/Checks/_ManageCheckItems.cshtml", manager);
            }
            return PartialView("_DeleteItem");
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

        private async Task ChangeCheckSum(int checkId, double sum)
        {
            var check = await _context.Checks.FirstOrDefaultAsync(m => m.Id == checkId);

            if(check is not null)
            {
                check.Sum += sum;
                await _context.SaveChangesAsync();
            }
        }
    }
}
