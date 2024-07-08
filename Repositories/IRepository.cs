using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IRepository<T> where T : DbObject
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Insert(T obj);
        Task Update(T obj);
        Task Delete(int id);
    }
}
