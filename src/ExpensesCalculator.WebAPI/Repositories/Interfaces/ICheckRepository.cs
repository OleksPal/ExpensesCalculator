using ExpensesCalculator.WebAPI.Models;

namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface ICheckRepository : IGenericRepository<Check>
{
    Task<ICollection<Check>> GetAllDayChecks(Guid dayExpensesId);
}
