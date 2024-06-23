using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class DayExpensesRepository : IDayExpensesRepository
    {
        private readonly ExpensesContext _context;
        private readonly string _requestorName;

        public DayExpensesRepository(ExpensesContext context, string requestorName)
        {
            _context = context;
            _requestorName = requestorName;
        }

        public async Task<IEnumerable<DayExpenses>> GetAll()
        {
            return await _context.Days.Where(d => d.PeopleWithAccess.Contains(_requestorName)).ToListAsync();
        }

        public async Task<DayExpenses> GetById(int id)
        {
            return await _context.Days.Where(d => d.PeopleWithAccess.Contains(_requestorName))
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task Insert(DayExpenses dayExpenses)
        {
            await _context.Days.AddAsync(dayExpenses);
            await _context.SaveChangesAsync();
        }

        public async Task Update(DayExpenses dayExpenses)
        {
            if (dayExpenses.PeopleWithAccess.Contains(_requestorName))
            {
                _context.Days.Update(dayExpenses);
                await _context.SaveChangesAsync();
            }            
        }

        public async Task Delete(int id)
        {
            var dayToDelete = await _context.Days.FindAsync(id);

            if (dayToDelete is not null && dayToDelete.PeopleWithAccess.Contains(_requestorName)) 
            {
                _context.Days.Remove(dayToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
