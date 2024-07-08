using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class DayExpensesRepository : IDayExpensesRepository
    {
        private readonly ExpensesContext _context;

        public DayExpensesRepository(ExpensesContext context)
        {
            _context = context;
        }

        public async Task<ICollection<DayExpenses>> GetAll()
        {
            return await _context.Days.ToListAsync();
        }

        public async Task<DayExpenses> GetById(int id)
        {
            return await _context.Days.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DayExpenses> Insert(DayExpenses dayExpenses)
        {
            await _context.Days.AddAsync(dayExpenses);
            await _context.SaveChangesAsync();

            return dayExpenses;
        }

        public async Task<DayExpenses> Update(DayExpenses dayExpenses)
        {
            _context.Days.Update(dayExpenses);
            await _context.SaveChangesAsync();

            return dayExpenses;
        }

        public async Task<DayExpenses> Delete(int id)
        {
            var dayToDelete = await _context.Days.FindAsync(id);

            if (dayToDelete is not null) 
            {
                _context.Days.Remove(dayToDelete);
                await _context.SaveChangesAsync();
            }

            return dayToDelete;
        }
    }
}
