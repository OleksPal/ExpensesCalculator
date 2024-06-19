using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IRepository
    {
        public IEnumerable<DbObject> GetAll();
        public DbObject GetById(int id);
        public void Insert(DbObject obj);
        public void Update(DbObject obj);
        public void Delete(int id);
    }
}
