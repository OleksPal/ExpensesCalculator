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

        public async Task<IEnumerable<Check>> GetAll(int dayExpensesId)
        {
            var dayExpenses = await _context.Days.Include(d => d.Checks).FirstOrDefaultAsync(d => d.Id == dayExpensesId);

            return (dayExpenses is not null) ? dayExpenses.Checks : new List<Check>();
        }

        public async Task<IEnumerable<Check>> GetAllWithItems(int dayExpensesId)
        {
            var checks = GetAll(dayExpensesId).Result.ToList();

            for (int i = 0; i < checks.Count(); i++) 
            {
                checks[i] = _context.Checks.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == checks[i].Id).Result;
            }

            return checks;
        }

        public async Task<Check> GetById(int id)
        {
            return await _context.Checks.FindAsync(id);
        }

        public async Task<Check> GetByIdWithItems(int id)
        {
            return await _context.Checks.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Insert(Check check, int dayExpensesId)
        {
            var dayExpenses = await _context.Days.Include(d => d.Checks)
                    .FirstOrDefaultAsync(d => d.Id == dayExpensesId);

            if (dayExpenses is not null)
            {
                _context.Checks.Add(check);
                dayExpenses.Checks.Add(check);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(Check check)
        {
            var checkToUpdate = await GetById(check.Id);

            if (checkToUpdate is not null)
            {
                _context.Checks.Update(check);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var checkToDelete = await GetByIdWithItems(id);

            if (checkToDelete is not null)
            {
                _context.Checks.Remove(checkToDelete);
                _context.Items.RemoveRange(checkToDelete.Items);
                await _context.SaveChangesAsync();
            }
        }
    }
}
