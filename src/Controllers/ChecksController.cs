using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class ChecksController : LanguageController
    {
        private readonly ICheckService _checkService;

        public ChecksController(ICheckService checkService)
        {
            _checkService = checkService;
        }

        [HttpGet]
        public async Task<JsonResult> GetCheckById(int id)
        {
            var check = await _checkService.GetCheckById(id);

            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return Json(check, options);
        }

        // GET: Checks/CreateCheck?dayExpensesId=1
        [HttpGet]
        public async Task<IActionResult> CreateCheck(int dayExpensesId)
        {
            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(dayExpensesId);
            ViewData["DayExpensesId"] = dayExpensesId;

            return PartialView("_CreateCheck");
        }

        // GET: Checks/EditCheck/5
        [HttpGet]
        public async Task<IActionResult> EditCheck(int? id)
        {
            if (id is null)
                return NotFound();

            var check = await _checkService.GetCheckById((int)id);

            if (check is null)
                return NotFound();

            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(check.DayExpensesId);

            return PartialView("_EditCheck", check);
        }

        // GET: Checks/DeleteCheck/5
        [HttpGet]
        public async Task<IActionResult> DeleteCheck(int? id)
        {
            if (id is null)
                return NotFound();

            var check = await _checkService.GetCheckById((int)id);

            if (check is null)
                return NotFound();

            return PartialView("_DeleteCheck", check);
        }

        // GET: Checks/GetCheckItemsManager/5
        [HttpGet]
        public async Task<IActionResult> GetCheckItemsManager(int id)
        {
            var check = await _checkService.GetCheckByIdWithItems(id);

            if (check is null)
                return NotFound();

            return PartialView("_ManageCheckItems", check);
        }

        // POST: Checks/Create?dayExpensesId=1
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DayExpensesId,Payer,Sum,Location,Items,Id")] Check check)
        {
            check = await _checkService.SetDayExpenses(check);
            ModelState.Clear();

            if (TryValidateModel(check))
            {
                var newCheck = await _checkService.AddCheckRCheck(check);
                return RedirectToAction(nameof(GetCheckById), new { id = newCheck.Id });
            }

            ViewData["DayExpensesId"] = check.DayExpensesId;
            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(check.DayExpensesId);

            return PartialView("_CreateCheck", check);
        }

        // POST: Checks/Edit/5?dayExpensesId=1
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("DayExpensesId,Payer,Sum,Location,Items,Id")] Check check)
        {
            check = await _checkService.SetDayExpenses(check);
            ModelState.Clear();

            if (TryValidateModel(check))
            {
                var model = await _checkService.EditCheck(check);

                return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", model);
            }

            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(check.DayExpensesId);

            return PartialView("_EditCheck", check);
        }

        // POST: Checks/Delete/5?dayExpensesId=1
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _checkService.DeleteCheck(id);

            return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", model);
        }
    }
}
