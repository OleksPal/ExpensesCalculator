using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Newtonsoft.Json;

namespace ExpensesCalculator.Controllers
{
    public class DayExpensesController : Controller
    {
        private readonly ExpensesContext _context;

        public DayExpensesController(ExpensesContext context)
        {
            _context = context;
        }

        // GET: DayExpenses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Days.ToListAsync());
        }

        // GET: DayExpenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _context.Days.Include(d => d.Checks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (day == null)
            {
                return NotFound();
            }

            var dayExpenses = new DayExpensesViewModel();
            dayExpenses.DayExpensesId = day.Id;
            dayExpenses.Date = day.Date;
            dayExpenses.Checks = day.Checks;

            for (int i = 0; i < dayExpenses.Checks.Count; i++)
            {
                var check = await _context.Checks.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == dayExpenses.Checks[i].Id);
                if (check is not null)
                    dayExpenses.Checks[i] = check;
            }

            var json = JsonConvert.SerializeObject(dayExpenses);
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "MyJSON2.json")))
            {
                outputFile.WriteLine(json);
            }

            return View(dayExpenses);
        }

        // GET: DayExpenses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DayExpenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Id")] DayExpenses dayExpenses)
        {
            dayExpenses.Checks = new List<Check>();
            ModelState.ClearValidationState(nameof(DayExpenses));
            if (!TryValidateModel(nameof(DayExpenses)))
            {
                _context.Add(dayExpenses);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dayExpenses);
        }

        // GET: DayExpenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dayExpenses = await _context.Days.FindAsync(id);
            if (dayExpenses == null)
            {
                return NotFound();
            }
            return View(dayExpenses);
        }

        // POST: DayExpenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Date,Id")] DayExpenses dayExpenses)
        {
            if (id != dayExpenses.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dayExpenses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DayExpensesExists(dayExpenses.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dayExpenses);
        }

        // GET: DayExpenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dayExpenses = await _context.Days
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dayExpenses == null)
            {
                return NotFound();
            }

            return View(dayExpenses);
        }

        // POST: DayExpenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dayExpenses = await _context.Days.FindAsync(id);
            if (dayExpenses != null)
            {
                _context.Days.Remove(dayExpenses);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DayExpensesExists(int id)
        {
            return _context.Days.Any(e => e.Id == id);
        }
    }
}
