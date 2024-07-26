using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;

namespace ExpensesCalculator.Repositories
{
    public class DayExpensesRepository : GenericRepository<DayExpenses>, IDayExpensesRepository
    {
        public DayExpensesRepository(ExpensesContext context) : base(context) { }
    }
}
