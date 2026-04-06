using ExpensesCalculator.WebAPI.Models;

namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface ICheckRepository : IGenericRepository<Check>
{
    Task<Check[]> GetAllDayChecks(Guid dayExpensesId);
}
