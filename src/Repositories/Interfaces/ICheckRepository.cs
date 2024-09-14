using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories.Interfaces
{
    public interface ICheckRepository : IGenericRepository<Check>
    {
        Task<ICollection<Check>> GetAllDayChecks(Guid dayExpensesId);
    }
}
