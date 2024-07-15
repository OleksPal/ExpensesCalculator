using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class CheckRepository : GenericRepository<Check>, ICheckRepository
    {
        public CheckRepository(ExpensesContext context) : base(context) { }

        public async Task<ICollection<Check>> GetAllDayChecks(int dayExpensesId)
        {
            var checks = await _context.Checks.Where(c => c.DayExpensesId == dayExpensesId).ToListAsync();

            return (checks is not null) ? checks : new List<Check>();
        }

        public override async Task<Check> GetById(int id)
        {
            var check = await _context.Checks.FirstOrDefaultAsync(i => i.Id == id);
            _context.ChangeTracker.Clear();
            return check;
        }
    }
}
