using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Services
{
    public class ExpensesService
    {
        private readonly ExpensesContext _context;

        public ExpensesService(ExpensesContext context)
        {
            _context = context;
        }

        public IEnumerable<DayExpenses> GetAll()
        {
            return _context.Days.AsNoTracking().ToList();
        }

        public DayExpenses? GetById(int id)
        {
            return _context.Days.Include(d => d.Checks).AsNoTracking().FirstOrDefault(d => d.Id == id);
        }

        public DayExpenses Create(DayExpenses dayExpenses)
        {
            _context.Days.Add(dayExpenses);
            _context.SaveChanges();

            return dayExpenses;
        }

        public void UpdateDayExpenses(int dayId, DayExpenses newDayExpenses)
        {
            var dayToUpdate = _context.Days.Find(dayId);

            if (dayToUpdate != null)
            {
                dayToUpdate = newDayExpenses;
                _context.SaveChanges();
            }
        }

        public void DeleteById(int dayId)
        {
            var dayToDelete = _context.Days.Find(dayId);

            if (dayToDelete != null)
            {
                _context.Days.Remove(dayToDelete);
                _context.SaveChanges();
            }
        }
    }
}
