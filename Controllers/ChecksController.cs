using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class ChecksController : Controller
    {
        private readonly ExpensesContext _context;

        public ChecksController(ExpensesContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateCheck(int dayExpensesId)
        {
            var dayExpenses = await _context.Days.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dayExpensesId);
            if (dayExpenses is not null)
            {
                List<SelectListItem> optionList = new List<SelectListItem>();
                foreach (var participant in dayExpenses.Participants)
                {
                    optionList.Add(new SelectListItem { Text = participant, Value = participant });
                }
                ViewData["Participants"] = new SelectList(optionList, "Value", "Text");
            }

            ViewData["DayExpensesId"] = dayExpensesId;

            return PartialView("_CreateCheck");
        }

        public async Task<IActionResult> EditCheck(int? id, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var check = await _context.Checks.FirstOrDefaultAsync(m => m.Id == id);
            if (check is null)
            {
                return NotFound();
            }

            ViewData["DayExpensesId"] = dayExpensesId;

            var dayExpenses = await _context.Days.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dayExpensesId);
            if (dayExpenses is not null)
            {
                List<SelectListItem> optionList = new List<SelectListItem>();
                foreach (var participant in dayExpenses.Participants)
                {
                    optionList.Add(new SelectListItem { Text = participant, Value = participant });
                }
                ViewData["Participants"] = new SelectList(optionList, "Value", "Text");
            }
            return PartialView("_EditCheck", check);
        }

        public async Task<IActionResult> DeleteCheck(int? id, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var check = await _context.Checks.FirstOrDefaultAsync(m => m.Id == id);
            if (check is null)
            {
                return NotFound();
            }

            return PartialView("_DeleteCheck", check);
        }

        public async Task<IActionResult> GetCheckItemsManager(int id, int dayExpensesId)
        {
            var check = await _context.Checks.Include(c => c.Items)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (check is null)
            {
                return NotFound();
            }

            var manager = new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
            return PartialView("_ManageCheckItems", manager);
        }

        // POST: Checks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Payer,Sum,Location,VerificationPath,Id")] Check check, int dayExpensesId)
        {
            check.Items = new List<Item>();
            ModelState.ClearValidationState(nameof(Check));
            if (!TryValidateModel(nameof(Check)))
            {
                var dayExpenses = await _context.Days.Include(d => d.Checks)
                    .FirstOrDefaultAsync(d => d.Id == dayExpensesId);

                if (dayExpenses is null)
                    return NotFound();

                _context.Checks.Add(check);
                dayExpenses.Checks.Add(check);
                await _context.SaveChangesAsync();

                var manager = new ManageDayExpensesChecksViewModel { Checks = dayExpenses.Checks, DayExpensesId = dayExpensesId };
                return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", manager);
            }
            return PartialView("_CreateCheck");
        }

        // POST: Checks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Payer,Sum,Location,VerificationPath,Id")] Check check, int dayExpensesId)
        {
            if (id != check.Id)
            {
                return NotFound();
            }

            if (check.Items is null) 
            {
                var oldCheck = await _context.Checks.Include(c => c.Items).AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (oldCheck is null)
                    return NotFound();

                check.Items = oldCheck.Items;
                check.Sum = oldCheck.Sum;
            }

            ModelState.ClearValidationState(nameof(Check));
            if (!TryValidateModel(nameof(Check)))
            {
                try
                {
                    _context.Update(check);
                    await _context.SaveChangesAsync();

                    var dayExpenses = await _context.Days.Include(d => d.Checks)
                    .FirstOrDefaultAsync(d => d.Id == dayExpensesId);

                    if (dayExpenses is null)
                        return NotFound();

                    dayExpenses.Checks.Add(check);
                    var manager = new ManageDayExpensesChecksViewModel { Checks = dayExpenses.Checks, DayExpensesId = dayExpensesId };
                    return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", manager);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckExists(check.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return PartialView("_EditCheck");
        }

        // POST: Checks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var check = await _context.Checks.Include(c => c.Items).AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

            if (check is null)
                return NoContent();

            _context.Items.RemoveRange(check.Items);

            if (check != null)
            {
                _context.Checks.Remove(check);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), nameof(DayExpenses));
        }

        private bool CheckExists(int id)
        {
            return _context.Checks.Any(e => e.Id == id);
        }
    }
}
