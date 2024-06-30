using AspNetCore;
using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class DayExpensesController : Controller
    {
        private readonly IDayExpensesService _dayExpensesService;

        public DayExpensesController(IDayExpensesService dayExpensesService)
        { 
            _dayExpensesService = dayExpensesService;
            _dayExpensesService.RequestorName = User.Identity.Name;
        }

        // GET: DayExpenses
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUsersName = User.Identity.Name;
            List<DayExpenses> days = new List<DayExpenses>();

            if (currentUsersName is not null)
            {
                var collection = await _dayExpensesService.GetAllDays();
                days = collection.ToList();
            }

            return View(days);
        }

        // GET: DayExpenses/CreateDayExpenses
        [HttpGet]
        public IActionResult CreateDayExpenses()
        {
            ViewData["CurrentUsersName"] = User.Identity.Name;
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

            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day == null)
            {
                return NotFound();
            }

            ViewData["CurrentUsersName"] = User.Identity.Name;
            ViewBag.FormatParticipantNames = _dayExpensesService.GetFormatParticipantsNames(day.Id);

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

            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day == null)
            {
                return NotFound();
            }

            ViewBag.FormatParticipantNames = _dayExpensesService.GetFormatParticipantsNames(day.Id);

            return PartialView("_DeleteDayExpenses", day);
        }

        // GET: DayExpenses/ShareDayExpenses/5
        [HttpGet]
        public async Task<IActionResult> ShareDayExpenses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day == null)
            {
                return NotFound();
            }

            ViewData["CurrentUsersName"] = User.Identity.Name;
            ViewBag.FormatParticipantNames = _dayExpensesService.GetFormatParticipantsNames(day.Id);

            return PartialView("_ShareDayExpenses", day);
        }

        // GET: DayExpenses/CalculateExpenses/5
        [HttpGet]
        public async Task<IActionResult> CalculateExpenses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dayExpensesCalculation = await _dayExpensesService.GetCalculationForDayExpenses((int)id);

            if (dayExpensesCalculation == null)
            {
                return NotFound();
            }

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

            var dayExpenses = await _dayExpensesService.GetFullDayExpensesById((int)id);

            if (dayExpenses is null)
            {
                return NotFound();
            }

            return View("ShowChecks", dayExpenses);
        }

        // POST: DayExpenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PeopleWithAccess,Participants,Date,Id")] DayExpenses dayExpenses)
        {
            var model = await _dayExpensesService.AddDayExpenses(dayExpenses);
            return View(model);
        }

        // POST: DayExpenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PeopleWithAccess,Participants,Date,Id")] DayExpenses dayExpenses)
        {            
            if (id != dayExpenses.Id)
            {
                return NotFound();
            }

            ModelState.ClearValidationState(nameof(DayExpenses));
            if (!TryValidateModel(nameof(DayExpenses)))
            {
                try
                {
                    var model = await _dayExpensesService.EditDayExpenses(dayExpenses);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dayExpensesService.DayExpensesExists(dayExpenses.Id))
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
            await _dayExpensesService.DeleteDayExpenses(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: DayExpenses/Share/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(int id, string newUserWithAccess)
        {
            var response = await _dayExpensesService.ChangeDayExpensesAccess(id, newUserWithAccess);
            if (response is null)
                return NotFound();
            else
                return Content(response);        
        }   
    }
}
