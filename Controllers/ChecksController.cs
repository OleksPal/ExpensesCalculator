using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ExpensesCalculator.Repositories;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class ChecksController : Controller
    {
        private readonly ExpensesContext _context;
        private readonly CheckRepository repository;

        public ChecksController(ExpensesContext context)
        {
            _context = context;
            repository = new CheckRepository(context);
        }

        // GET: Checks/CreateCheck?dayExpensesId=1
        [HttpGet]
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

        // GET: Checks/EditCheck/5?dayExpensesId=1
        [HttpGet]
        public async Task<IActionResult> EditCheck(int? id, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var check = await repository.GetById((int)id);

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

        // GET: Checks/DeleteCheck/5?dayExpensesId=1
        [HttpGet]
        public async Task<IActionResult> DeleteCheck(int? id, int dayExpensesId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var check = await repository.GetById((int)id);
            if (check is null)
            {
                return NotFound();
            }

            ViewData["DayExpensesId"] = dayExpensesId;
            return PartialView("_DeleteCheck", check);
        }

        // GET: Checks/GetCheckItemsManager/5?dayExpensesId=1
        [HttpGet]
        public async Task<IActionResult> GetCheckItemsManager(int id, int dayExpensesId)
        {
            var check = await repository.GetByIdWithItems(id);

            if (check is null)
            {
                return NotFound();
            }

            var manager = new ManageCheckItemsViewModel { Check = check, DayExpensesId = dayExpensesId };
            return PartialView("_ManageCheckItems", manager);
        }

        // POST: Checks/Create?dayExpensesId=1
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Payer,Sum,Location,Items,Id")] Check check, int dayExpensesId)
        {
            check.Items = new List<Item>();
            ModelState.ClearValidationState(nameof(Check));
            if (!TryValidateModel(nameof(Check)))
            {
                var dayExpenses = await _context.Days.Include(d => d.Checks)
                    .FirstOrDefaultAsync(d => d.Id == dayExpensesId);

                if (dayExpenses is null)
                    return NotFound();

                await repository.Insert(check, dayExpensesId);

                return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", dayExpenses);
            }
            return PartialView("_CreateCheck");
        }

        // POST: Checks/Edit/5?dayExpensesId=1
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Payer,Sum,Location,Items,Id")] Check check, int dayExpensesId)
        {
            if (id != check.Id)
            {
                return NotFound();
            }

            ModelState.ClearValidationState(nameof(Check));
            if (!TryValidateModel(nameof(Check)))
            {
                try
                {
                    await repository.Update(check);

                    var dayExpenses = await _context.Days.Include(d => d.Checks)
                    .FirstOrDefaultAsync(d => d.Id == dayExpensesId);
                    return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", dayExpenses);
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

        // POST: Checks/Delete/5?dayExpensesId=1
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int dayExpensesId)
        {
            await repository.Delete(id);

            var dayExpenses = await _context.Days.Include(d => d.Checks)
                    .FirstOrDefaultAsync(d => d.Id == dayExpensesId);

            if (dayExpenses is null)
                return NotFound();

            return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", dayExpenses);
        }

        private bool CheckExists(int id)
        {
            return _context.Checks.Any(e => e.Id == id);
        }
    }
}
