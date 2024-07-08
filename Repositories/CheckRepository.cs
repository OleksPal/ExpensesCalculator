using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class CheckRepository : ICheckRepository
    {
        private readonly ExpensesContext _context;
        public CheckRepository(ExpensesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Check>> GetAllDayChecks(int dayExpensesId)
        {
            var checks = await _context.Checks.Where(c => c.DayExpensesId == dayExpensesId).ToListAsync();

            return (checks is not null) ? checks : new List<Check>();
        }

        public async Task<Check> GetById(int id)
        {
            return await _context.Checks.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Check> Insert(Check check)
        {
            _context.Checks.Add(check);
            await _context.SaveChangesAsync();

            return check;
        }

        public async Task<Check> Update(Check check)
        {
            var checkToUpdate = await _context.Checks.AsNoTracking().FirstOrDefaultAsync(i => i.Id == check.Id);

            if (checkToUpdate is not null)
            {
                _context.Checks.Update(check);
                await _context.SaveChangesAsync();
            }

            return check;
        }

        public async Task<Check> Delete(int id)
        {
            var checkToDelete = await GetById(id);

            if (checkToDelete is not null)
            {
                _context.Checks.Remove(checkToDelete);
                await _context.SaveChangesAsync();
            }

            return checkToDelete;
        }
    }
}
