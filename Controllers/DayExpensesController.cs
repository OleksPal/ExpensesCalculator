using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

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
            var days = await _context.Days.Include(d => d.Checks).ToListAsync();
            ViewData["FormattedDayParticipants"] = new List<string>();
            List<string> formattedDayParticipants = new List<string>();

            foreach (var day in days)
            {
                for (int i = 0; i < day.Checks.Count; i++) 
                {
                    var check = await _context.Checks.Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.Id == day.Checks[i].Id);
                    if (check is not null)
                        day.Checks[i] = check;
                }
                (ViewData["FormattedDayParticipants"] as List<string>).Add(GetFormatParticipantsNames(day.Participants));
            }           

            return View(new DayExpensesViewModel { Days = days });
        }

        public async Task<IActionResult> EditDayExpenses(int id, string act)
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

            for (int i = 0; i < day.Checks.Count; i++)
            {
                var check = await _context.Checks.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == day.Checks[i].Id);
                if (check is not null)
                    day.Checks[i] = check;
            }

            ViewBag.FormatParticipantNames = GetFormatParticipantsNames(day.Participants);

            if(act == "Edit")
            {

                return PartialView("_EditDayExpenses", day);
            }
                
            else
                return PartialView("_DeleteDayExpenses", day);
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

            for (int i = 0; i < day.Checks.Count; i++)
            {
                var check = await _context.Checks.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == day.Checks[i].Id);
                if (check is not null)
                    day.Checks[i] = check;
            }

            ViewBag.FormatParticipantNames = GetFormatParticipantsNames(day.Participants);

            return View(day);
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

        // GET: DayExpenses/CalculateExpenses/5
        public async Task<IActionResult> CalculateExpenses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dayExpenses = await _context.Days.Include(d => d.Checks).FirstOrDefaultAsync(d => d.Id == id);

            if (dayExpenses == null)
            {
                return NotFound();
            }

            var dayExpensesCalculation = new DayExpensesCalculationViewModel 
            { DayExpensesId = dayExpenses.Id, Participants = dayExpenses.Participants };

            dayExpensesCalculation.Checks = new List<Check>();
            dayExpensesCalculation.UserTransactions = new Dictionary<string[], double>();
            for(int i = 0; i < dayExpenses.Checks.Count; i++)
            {
                var checkWithItems = await _context.Checks.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == dayExpenses.Checks[i].Id);

                if (checkWithItems is not null)
                    dayExpensesCalculation.Checks.Add(checkWithItems);
            }

            Dictionary<string[], double> transactions = CalculateTransactionList(dayExpensesCalculation.Participants,
                dayExpensesCalculation.Checks);
            var optimizedTransactions = OptimizeTransactions(transactions);
            dayExpensesCalculation.UserTransactions = optimizedTransactions;          

            return View(dayExpensesCalculation);
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

        private Dictionary<string[], double> CalculateTransactionList(List<string> participants, List<Check> checks) 
        {
            Dictionary<string[], double> userTransactions = new();
            foreach (var participant in participants)
            {
                foreach (var check in checks)
                {
                    foreach (var item in check.Items)
                    {
                        if (participant != check.Payer)
                        {
                            var transactionKey = new string[2] { participant, check.Payer };
                            var transactionValue = Math.Round(item.Price / item.Users.Count, 2);

                            if (!userTransactions.Keys.Any(key => key.SequenceEqual(transactionKey)))
                                userTransactions.Add(transactionKey, transactionValue);
                            else
                            {
                                var oldValue = userTransactions.Where(x => x.Key.SequenceEqual(transactionKey))
                                    .Select(x => x.Value).FirstOrDefault();
                                var oldKey = userTransactions.Where(x => x.Key.SequenceEqual(transactionKey))
                                    .Select(x => x.Key).FirstOrDefault();
                                userTransactions.Remove(oldKey);
                                userTransactions.Add(transactionKey, oldValue + transactionValue);
                            }                                
                        }
                    }
                }
            }

            return userTransactions;
        }

        private Dictionary<string[], double> OptimizeTransactions(Dictionary<string[], double> transactionList)
        {
            var transactionKeyList = transactionList.Keys.ToList();
            for (int i = 0; i < transactionKeyList.Count; i++)
            {
                for (int j = 1; j < transactionKeyList.Count; j++)
                {
                    if (transactionKeyList[i][0] == transactionKeyList[j][1] &&
                        transactionKeyList[i][1] == transactionKeyList[j][0])
                    {
                        if (transactionList[transactionKeyList[i]] > transactionList[transactionKeyList[j]])
                        {
                            transactionList[transactionKeyList[i]] -= transactionList[transactionKeyList[j]];
                            transactionList.Remove(transactionKeyList[j]);
                        }
                        else if (transactionList[transactionKeyList[i]] < transactionList[transactionKeyList[j]])
                        {
                            transactionList[transactionKeyList[j]] -= transactionList[transactionKeyList[i]];
                            transactionList.Remove(transactionKeyList[i]);
                        }
                        else
                        {
                            transactionList.Remove(transactionKeyList[i]);
                            transactionList.Remove(transactionKeyList[j]);
                        }
                    }
                }
            }

            return transactionList;
        }
    }
}
