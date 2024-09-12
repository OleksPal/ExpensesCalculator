using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : DbObject
    {
        protected readonly ExpensesContext _context;

        public GenericRepository(ExpensesContext context)
        {
            _context = context;
        }

        public virtual async Task<ICollection<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> Insert(T obj)
        {
            await _context.Set<T>().AddAsync(obj);
            await _context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<T> Update(T obj)
        {
            _context.Set<T>().Update(obj);
            await _context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<T> Delete(int id)
        {
            var obj = await GetById(id);

            if (obj is not null) 
            {
                _context.Set<T>().Remove(obj);
                await _context.SaveChangesAsync();
            }            

            return obj;
        }
    }
}
