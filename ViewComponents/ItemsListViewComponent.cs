using Microsoft.AspNetCore.Mvc;
using ExpensesCalculator.Models;
using ExpensesCalculator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpensesCalculator.ViewComponents
{
    public class ItemsListViewComponent : ViewComponent
    {
        private readonly ExpensesContext _context;

        public ItemsListViewComponent(ExpensesContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int checkId)
        {
            var check = await _context.Checks.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == checkId);

            if (check is null)
            {
                return View(new List<Item>());
            }

            return View(check.Items);
        }
    }
}
