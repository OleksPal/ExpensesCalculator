using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class DayExpensesController : Controller
    {
        private readonly IDayExpensesService _dayExpensesService;

        public DayExpensesController(IDayExpensesService dayExpensesService)
        { 
            _dayExpensesService = dayExpensesService;
        }

        // GET: DayExpenses
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _dayExpensesService.RequestorName = User.Identity.Name;

            var collection = await _dayExpensesService.GetAllDays();
            List<DayExpenses> days = collection.ToList();

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

            _dayExpensesService.RequestorName = User.Identity.Name;
            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day == null)
            {
                return NotFound();
            }

            ViewData["CurrentUsersName"] = User.Identity.Name;
            ViewBag.FormatParticipantNames = await _dayExpensesService.GetFormatParticipantsNames(day.Id);

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

            _dayExpensesService.RequestorName = User.Identity.Name;
            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day == null)
            {
                return NotFound();
            }

            ViewBag.FormatParticipantNames = await _dayExpensesService.GetFormatParticipantsNames(day.Id);

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

            _dayExpensesService.RequestorName = User.Identity.Name;
            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day == null)
            {
                return NotFound();
            }

            ViewData["CurrentUsersName"] = User.Identity.Name;
            ViewBag.FormatParticipantNames = await _dayExpensesService.GetFormatParticipantsNames(day.Id);

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

            _dayExpensesService.RequestorName = User.Identity.Name;
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

            _dayExpensesService.RequestorName = User.Identity.Name;
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
        public async Task<IActionResult> Create([Bind("PeopleWithAccessList,ParticipantsList,Date,Id")] DayExpenses dayExpenses)
        {
            if (dayExpenses.ParticipantsList.ToList()[0] is null) 
                ModelState.AddModelError("ParticipantsList", "Add some participants!");
            if (ModelState.IsValid)
            {
                await _dayExpensesService.AddDayExpenses(dayExpenses);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["CurrentUsersName"] = User.Identity.Name;
                return PartialView("_CreateDayExpenses", dayExpenses);
            }             
        }

        // POST: DayExpenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PeopleWithAccessList,ParticipantsList,Date,Id")] DayExpenses dayExpenses)
        {            
            if (id != dayExpenses.Id)
            {
                return NotFound();
            }

            _dayExpensesService.RequestorName = User.Identity.Name;

            if (dayExpenses.ParticipantsList.ToList()[0] is null)
                ModelState.AddModelError("ParticipantsList", "Add some participants!");
            if (ModelState.IsValid)
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
            ViewBag.FormatParticipantNames = await _dayExpensesService.GetFormatParticipantsNames(dayExpenses.Id);
            return PartialView("_EditDayExpenses", dayExpenses);
        }

        // POST: DayExpenses/Delete/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _dayExpensesService.RequestorName = User.Identity.Name;
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
            _dayExpensesService.RequestorName = User.Identity.Name;
            var response = await _dayExpensesService.ChangeDayExpensesAccess(id, newUserWithAccess);
            if (response is null)
                return NotFound();
            else
                return Content(response);        
        }   
    }
}
