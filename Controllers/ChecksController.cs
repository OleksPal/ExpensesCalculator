using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Controllers
{
    [Authorize]
    public class ChecksController : Controller
    {
        private readonly ICheckService _checkService;

        public ChecksController(ICheckService checkService)
        {
            _checkService = checkService;
        }

        // GET: Checks/CreateCheck?dayExpensesId=1
        [HttpGet]
        public async Task<IActionResult> CreateCheck(int dayExpensesId)
        {
            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(dayExpensesId);
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

            var check = await _checkService.GetCheckById((int)id);

            if (check is null)
            {
                return NotFound();
            }

            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(dayExpensesId);
            ViewData["DayExpensesId"] = dayExpensesId;

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

            var check = await _checkService.GetCheckById((int)id);

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
            var check = await _checkService.GetCheckByIdWithItems(id);

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
            ModelState.ClearValidationState(nameof(Check));
            if (!TryValidateModel(nameof(Check)))
            {
                var model = _checkService.AddCheck(check, dayExpensesId); 
                return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", model);
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
                    var model = await _checkService.EditCheck(check, dayExpensesId);
                    return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _checkService.CheckExists(check.Id))
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
            var model = await _checkService.DeleteCheck(id, dayExpensesId);
            return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", model);
        }
    }
}
