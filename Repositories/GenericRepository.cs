using ExpensesCalculator.Data;
using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public class GenericRepository : IRepository
    {
        protected readonly ExpensesContext _context;

        public GenericRepository(ExpensesContext context)
        {
            _context = context;
        }

        public IEnumerable<DbObject> GetAll()
        {
            return _context.Set<DbObject>().ToList();
        }

        public DbObject GetById(int id)
        {
            return _context.Set<DbObject>().Find(id);
        }

        public void Insert(DbObject obj)
        {
            _context.Set<DbObject>().Add(obj);
        }

        public void Update(DbObject obj)
        {
            _context.Set<DbObject>().Update(obj);
        }

        public void Delete(int id)
        {
            var obj = GetById(id);
            _context.Set<DbObject>().Remove(obj);
        }
    }
}
