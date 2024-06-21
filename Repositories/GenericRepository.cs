using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class GenericRepository : IRepository
    {
        protected readonly ExpensesContext _context;

        public GenericRepository(ExpensesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DbObject>> GetAll()
        {
            return await _context.Set<DbObject>().ToListAsync();
        }

        public async Task<DbObject> GetById(int id)
        {
            return await _context.Set<DbObject>().FindAsync(id);
        }

        public async Task Insert(DbObject obj)
        {
            await _context.Set<DbObject>().AddAsync(obj);
        }

        public async Task Update(DbObject obj)
        {
            _context.Set<DbObject>().Update(obj);
        }

        public async Task Delete(int id)
        {
            var obj = GetById(id);
            _context.Set<DbObject>().Remove(obj.Result);
        }
    }
}
