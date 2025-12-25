using ExpensesCalculator.WebAPI.Data;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
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

    public virtual async Task<T> GetById(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task Insert(T obj)
    {
        await _context.Set<T>().AddAsync(obj);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Update(T obj)
    {
        _context.Set<T>().Update(obj);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Delete(Guid id)
    {
        var obj = await GetById(id);

        if (obj is not null)
        {
            _context.Set<T>().Remove(obj);
            await _context.SaveChangesAsync();
        }
    }
}
