namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface IGenericRepository<T>
{
    Task<ICollection<T>> GetAll();
    Task<T> GetById(Guid id);
    Task Insert(T obj);
    Task Update(T obj);
    Task Delete(Guid id);
}
