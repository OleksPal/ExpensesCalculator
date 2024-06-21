using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IRepository<T> where T : DbObject
    {
        public Task<IEnumerable<T>> GetAll();
        public Task<T> GetById(int id);
        public Task Insert(T obj);
        public Task Update(T obj);
        public Task Delete(int id);
    }
}
