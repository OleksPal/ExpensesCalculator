using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;

namespace ExpensesCalculator.Controllers
{
    public class ChecksController : Controller
    {
        private readonly ExpensesContext _context;

        public ChecksController(ExpensesContext context)
        {
            _context = context;
        }

        // GET: Checks/Create
        [HttpGet]
        public IActionResult Create(int dayExpensesId)
        {
            if (Request.Headers.ContainsKey("Referer"))
                ViewData["PreviousUrl"] = Request.Headers["Referer"].ToString();

            ViewData["DayExpensesId"] = dayExpensesId;
            return View();
        }

        // POST: Checks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Sum,Location,VerificationPath,Id")] Check check, int dayExpensesId)
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
                return RedirectToAction(nameof(Index), nameof(DayExpenses));
            }
            return View(check);
        }

        // GET: Checks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var check = await _context.Checks.Include(c => c.Items)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (check == null)
            {
                return NotFound();
            }

            if (Request.Headers.ContainsKey("Referer"))
                ViewData["PreviousUrl"] = Request.Headers["Referer"].ToString();

            return View(check);
        }

        // POST: Checks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Sum,Location,VerificationPath,Id")] Check check)
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
                return RedirectToAction(nameof(Index), nameof(DayExpenses));
            }
            return View(check);
        }

        // GET: Checks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var check = await _context.Checks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (check == null)
            {
                return NotFound();
            }

            if (Request.Headers.ContainsKey("Referer"))
                ViewData["PreviousUrl"] = Request.Headers["Referer"].ToString();

            return View(check);
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
