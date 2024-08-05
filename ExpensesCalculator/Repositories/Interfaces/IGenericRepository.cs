using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : DbObject
    {
        Task<ICollection<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> Insert(T obj);
        Task<T> Update(T obj);
        Task<T> Delete(int id);
    }
}
