using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var days = await _context.Days.Include(d => d.Checks).ToListAsync();
            ViewData["FormattedDayParticipants"] = new List<string>();
            ViewData["CurrentUser"] = User.Identity.Name;
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

            return View(days);
        }

        // GET: DayExpenses/CreateDayExpenses
        [HttpGet]
        public IActionResult CreateDayExpenses()
        {
            return PartialView("_CreateDayExpenses");
        }

        // GET: DayExpenses/EditDayExpenses/5
        [HttpGet]
        public async Task<IActionResult> EditDayExpenses(int? id)
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

            ViewBag.FormatParticipantNames = GetFormatParticipantsNames(day.Participants);

            return PartialView("_EditDayExpenses", day);
        }

        // GET: DayExpenses/DeleteDayExpenses/5
        [HttpGet]
        public async Task<IActionResult> DeleteDayExpenses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _context.Days.FirstOrDefaultAsync(m => m.Id == id);

            if (day == null)
            {
                return NotFound();
            }

            ViewBag.FormatParticipantNames = GetFormatParticipantsNames(day.Participants);

            return PartialView("_DeleteDayExpenses", day);
        }

        // GET: DayExpenses/CalculateExpenses/5
        [HttpGet]
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
            
            for (int i = 0; i < dayExpenses.Checks.Count; i++)
            {
                var checkWithItems = await _context.Checks.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == dayExpenses.Checks[i].Id);

                if (checkWithItems is not null)
                    dayExpensesCalculation.Checks.Add(checkWithItems);
            }

            dayExpensesCalculation.AllUsersTrasactions = CalculateTransactionList(dayExpensesCalculation.Checks);

            var transactionsDictionary = new Dictionary<SenderRecipient, decimal>();
            foreach(var transaction in dayExpensesCalculation.AllUsersTrasactions)
            {
                if (!transactionsDictionary.Keys.Any(key => key.Equals(transaction.Subjects)))
                    transactionsDictionary.Add(transaction.Subjects, transaction.TransferAmount);
                else
                    transactionsDictionary[transaction.Subjects] += transaction.TransferAmount;                
            }

            var optimizedTransactions = OptimizeTransactions(transactionsDictionary);
            dayExpensesCalculation.OptimizedUserTransactions = optimizedTransactions;

            return View(dayExpensesCalculation);
        }

        // GET: DayExpenses/ShowChecks/5
        [HttpGet]
        public async Task<IActionResult> ShowChecks(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var dayExpenses = await _context.Days.Include(d => d.Checks).FirstOrDefaultAsync(d => d.Id == id);

            if (dayExpenses is null)
            {
                return NotFound();
            }

            var checks = new List<Check>();
            for (int i = 0; i < dayExpenses.Checks.Count; i++)
            {
                var checkWithItems = await _context.Checks.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == dayExpenses.Checks[i].Id);

                if (checkWithItems is not null)
                    checks.Add(checkWithItems);
            }

            return View("ShowChecks", dayExpenses);
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

        // POST: DayExpenses/Delete/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        private List<Transaction> CalculateTransactionList(List<Check> checks) 
        {
            List<Transaction> fullTransactionList = new();
            foreach (var check in checks)
            {
                Dictionary<SenderRecipient, decimal> userTransactions = new();
                foreach (var item in check.Items)
                {
                    foreach (var user in item.Users) 
                    {
                        if (user != check.Payer)
                        {
                            var transactionKey = new SenderRecipient(user, check.Payer);
                            decimal transactionValue = (decimal)(item.Price / item.Users.Count);

                            if (!userTransactions.Keys.Any(key => key.Equals(transactionKey)))
                                userTransactions.Add(transactionKey, transactionValue);
                            else
                                userTransactions[transactionKey] += transactionValue;                            
                        }
                    }                    
                }

                foreach(var transaction in userTransactions)
                {
                    var newTransaction = new Transaction();
                    newTransaction.CheckName = check.Location;
                    newTransaction.Subjects = transaction.Key;
                    newTransaction.TransferAmount = transaction.Value;
                    fullTransactionList.Add(newTransaction);
                }                
            }

            return fullTransactionList;
        }

        private Dictionary<SenderRecipient, decimal> OptimizeTransactions(Dictionary<SenderRecipient, decimal> transactionList)
        {
            var transactionKeyList = transactionList.Keys.ToList();
            for (int i = 0; i < transactionKeyList.Count; i++)
            {
                for (int j = 1; j < transactionKeyList.Count; j++)
                {
                    if (transactionKeyList[i].Sender == transactionKeyList[j].Recipient &&
                        transactionKeyList[i].Recipient == transactionKeyList[j].Sender)
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
