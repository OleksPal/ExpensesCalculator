using ExpensesCalculator.WebAPI.Data;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Repositories;

public class CheckRepository : GenericRepository<Check>, ICheckRepository
{
    public CheckRepository(ExpensesContext context) : base(context) { }

    public async Task<Check[]> GetAllDayChecks(Guid dayExpensesId)
    {
        return await _context.Checks
            .AsNoTracking()
            .Where(c => c.DayExpensesId == dayExpensesId)
            .ToArrayAsync();
    }
}
