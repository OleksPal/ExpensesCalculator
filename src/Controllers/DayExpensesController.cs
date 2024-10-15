using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            if (User.Identity.Name is not null) 
                _dayExpensesService.RequestorName = User.Identity.Name;

            var days = await _dayExpensesService.GetAllDays();

            return View(days);
        }

        // GET: DayExpenses
        [HttpGet]
        public async Task<JsonResult> GetAllDays()
        {
            if (User.Identity.Name is not null)
                _dayExpensesService.RequestorName = User.Identity.Name;

            var days = await _dayExpensesService.GetAllDays();

            return Json(days);
        }

        // GET: DayExpenses/CreateDayExpenses
        [HttpGet]
        public IActionResult CreateDayExpenses()
        {
            ViewData["CurrentUsersName"] = User.Identity.Name is not null ? User.Identity.Name : "Guest";

            return PartialView("_CreateDayExpenses");
        }

        // GET: DayExpenses/EditDayExpenses/5
        [HttpGet]
        public async Task<IActionResult> EditDayExpenses(int? id)
        {
            if (id is null)
                return NotFound();

            if (User.Identity.Name is not null)
                _dayExpensesService.RequestorName = User.Identity.Name;
            
            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day is null)
                return NotFound();

            ViewData["CurrentUsersName"] = User.Identity.Name is not null ? User.Identity.Name : "Guest";
            ViewData["FormatParticipantNames"] = await _dayExpensesService.GetFormatParticipantsNames(day.ParticipantsList);

            return PartialView("_EditDayExpenses", day);
        }

        // GET: DayExpenses/DeleteDayExpenses/5
        [HttpGet]
        public async Task<IActionResult> DeleteDayExpenses(int? id)
        {
            if (id is null)
                return NotFound();

            if (User.Identity.Name is not null)
                _dayExpensesService.RequestorName = User.Identity.Name;

            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day is null)
                return NotFound();

            ViewData["FormatParticipantNames"] = await _dayExpensesService.GetFormatParticipantsNames(day.ParticipantsList);

            return PartialView("_DeleteDayExpenses", day);
        }

        // GET: DayExpenses/ShareDayExpenses/5
        [HttpGet]
        public async Task<IActionResult> ShareDayExpenses(int? id)
        {
            if (id is null)
                return NotFound();

            if (User.Identity.Name is not null)
                _dayExpensesService.RequestorName = User.Identity.Name;

            var day = await _dayExpensesService.GetDayExpensesById((int)id);

            if (day is null)
                return NotFound();

            ViewData["CurrentUsersName"] = User.Identity.Name is not null ? User.Identity.Name : "Guest";
            ViewData["FormatParticipantNames"] = await _dayExpensesService.GetFormatParticipantsNames(day.ParticipantsList);

            return PartialView("_ShareDayExpenses", day);
        }

        // GET: DayExpenses/CalculateExpenses/5
        [HttpGet]
        public async Task<IActionResult> CalculateExpenses(int? id)
        {
            if (id is null)
                return NotFound();

            if (User.Identity.Name is not null)
                _dayExpensesService.RequestorName = User.Identity.Name;

            var dayExpensesCalculation = await _dayExpensesService.GetCalculationForDayExpenses((int)id);

            if (dayExpensesCalculation is null)
                return NotFound();

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

            if (User.Identity.Name is not null)
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
            if (dayExpenses.ParticipantsList.First() is null) 
                ModelState.AddModelError("ParticipantsList", "Add some participants");

            if (ModelState.IsValid)
            {
                await _dayExpensesService.AddDayExpenses(dayExpenses);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CurrentUsersName"] = User.Identity.Name is not null ? User.Identity.Name : "Guest";

            return PartialView("_CreateDayExpenses", dayExpenses);
        }

        // POST: DayExpenses/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("PeopleWithAccessList,ParticipantsList,Date,Id")] DayExpenses dayExpenses)
        {            
            if (User.Identity.Name is not null)
                _dayExpensesService.RequestorName = User.Identity.Name;

            if (dayExpenses.ParticipantsList.First() is null)
                ModelState.AddModelError("ParticipantsList", "Add some participants!");

            if (ModelState.IsValid)
            {
                await _dayExpensesService.EditDayExpenses(dayExpenses);

                return RedirectToAction(nameof(Index));
            }

            ViewData["FormatParticipantNames"] = await _dayExpensesService.GetFormatParticipantsNames(dayExpenses.ParticipantsList);

            return PartialView("_EditDayExpenses", dayExpenses);
        }

        // POST: DayExpenses/Delete/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.Identity.Name is not null)
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
            if (User.Identity.Name is not null)
                _dayExpensesService.RequestorName = User.Identity.Name;

            var response = await _dayExpensesService.ChangeDayExpensesAccess(id, newUserWithAccess);

            return response is not null ? Content(response) : NotFound();      
        }   
    }
}
