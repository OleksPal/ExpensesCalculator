using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IRepository
    {
        public Task<IEnumerable<DbObject>> GetAll();
        public Task<DbObject> GetById(int id);
        public Task Insert(DbObject obj);
        public Task Update(DbObject obj);
        public Task Delete(int id);
    }
}
