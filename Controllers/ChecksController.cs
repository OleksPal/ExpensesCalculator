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
        public async Task<IActionResult> Create([Bind("DayExpensesId,Payer,Sum,Location,Items,Id")] Check check)
        {
            if (ModelState.IsValid)
            {
                var model = await _checkService.AddCheck(check, check.DayExpensesId); 
                return PartialView("~/Views/DayExpenses/_ManageDayExpensesChecks.cshtml", model);
            }

            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(check.DayExpensesId);
            ViewData["DayExpensesId"] = check.DayExpensesId;

            return PartialView("_CreateCheck", check);
        }

        // POST: Checks/Edit/5?dayExpensesId=1
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DayExpensesId,Payer,Sum,Location,Items,Id")] Check check)
        {
            if (id != check.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var model = await _checkService.EditCheck(check, check.DayExpensesId);
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

            ViewData["Participants"] = await _checkService.GetAllAvailableCheckPayers(check.DayExpensesId);
            ViewData["DayExpensesId"] = check.DayExpensesId;

            return PartialView("_EditCheck", check);
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
