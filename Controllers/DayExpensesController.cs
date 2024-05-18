using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
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

            ViewBag.FormatParticipantNames = GetFormatParticipantsNames(day.Participants);

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
        public async Task<IActionResult> Create([Bind("Participants,Date,Id")] DayExpenses dayExpenses)
        {
            string rareNameList = dayExpenses.Participants[0];
            dayExpenses.Participants = (List<string>)GetParticipantListFromString(rareNameList);

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

            ViewBag.FormatParticipantNames = GetFormatParticipantsNames(dayExpenses.Participants);

            return View(dayExpenses);
        }

        // POST: DayExpenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Participants,Date,Id")] DayExpenses dayExpenses)
        {            
            if (id != dayExpenses.Id)
            {
                return NotFound();
            }

            string rareNameList = dayExpenses.Participants[0];
            dayExpenses.Participants = (List<string>)GetParticipantListFromString(rareNameList);

            dayExpenses.Checks = new List<Check>();
            ModelState.ClearValidationState(nameof(DayExpenses));
            if (!TryValidateModel(nameof(DayExpenses)))
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

            ViewBag.FormatParticipantNames = GetFormatParticipantsNames(dayExpenses.Participants);

            return View(dayExpenses);
        }

        // POST: DayExpenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dayExpenses = await _context.Days.Include(d => d.Checks).FirstOrDefaultAsync(d => d.Id == id);

            if (dayExpenses != null)
            {
                foreach(var check in dayExpenses.Checks)
                {
                    var checkToDelete = await _context.Checks.Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.Id == check.Id);

                    if(checkToDelete is not null)
                        _context.Items.RemoveRange(checkToDelete.Items);

                    _context.Checks.Remove(check);
                }
                _context.Days.Remove(dayExpenses);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DayExpensesExists(int id)
        {
            return _context.Days.Any(e => e.Id == id);
        }

        private string GetFormatParticipantsNames(List<string> participants)
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

        private IEnumerable<string> GetParticipantListFromString(string rareText)
        {
            Regex spacesAfterComma = new Regex(@",\s+"),
                bigSpaces = new Regex(@"\s+");
            rareText = bigSpaces.Replace(rareText, " ");
            rareText = spacesAfterComma.Replace(rareText, ",");

            List<string> participantList = rareText.Split(',').ToList();
            participantList = participantList.Select(p => p != null ? p.Trim() : null).ToList();

            return participantList;
        }
    }
}
