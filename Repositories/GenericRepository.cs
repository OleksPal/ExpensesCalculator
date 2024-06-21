using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : DbObject
    {
        protected readonly ExpensesContext _context;

        public GenericRepository(ExpensesContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task Insert(T obj)
        {
            await _context.Set<T>().AddAsync(obj);
        }

        public virtual async Task Update(T obj)
        {
            _context.Set<T>().Update(obj);
        }

        public virtual async Task Delete(int id)
        {
            var obj = GetById(id);
            _context.Set<T>().Remove(obj.Result);
        }
    }
}
